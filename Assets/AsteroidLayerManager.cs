﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AsteroidLayerManager : MonoBehaviour
{
    //[Serializable]
    //public class MinMaxSettings<T>
    //{
    //    public T min;
    //    public T max;
    //}

    //[Serializable]
    //public class MinMaxFloatSettings : MinMaxSettings<float> { }

    //[Serializable]
    //public class MinMaxIntSettings : MinMaxSettings<int> { }

    [Serializable]
    public class InterSpawnDelaySettings
    {
        public float min = 0.1f;
        public float max = 30.0f;
    }


    [Serializable]
    public class SpeedSettings
    {
        public float min = 0.1f;
        public float max = 2.0f;
    }

    [Serializable]
    public class RotationSettings
    {
        public float min = 5.0f;
        public float max = 90.0f;
    }

    [Serializable]
    public class ScaleSettings
    {
        public float min = 0.5f;
        public float max = 2.5f;
    }

    [Serializable]
    public class SpawnCountSettings
    {
        public int min = 1;
        public int max = 50;
    }

    [Serializable]
    public class MinMaxIntSettings
    {
        public int min;
        public int max;
    }

    [Serializable]
    public class Settings
    {
        public int sortingOrder = 0;

        //public static float minInterSpawnDelay = 0.1f, defaultInterSpawnDelay = 2.5f;
        //public static float minSpeed = 0.1f, defaultSpeed = 0.1f;
        //public static float minRotation = 5.0f, defaultRotation = 10.0f;
        //public static float minScale = 0.5f, defaultScale = 0.5f;
        //public static int minSpawnCount = 3, defaultSpawnCount = 10;

        //[Range(0.1f, 30.0f)]
        //public float interSpawnDelay = defaultInterSpawnDelay;

        public InterSpawnDelaySettings interSpawnDelay;

        //[Range(0.1f, 2.0f)]
        //public float speed = defaultSpeed;

        public SpeedSettings speed;

        //[Range(5.0f, 90.0f)]
        //public float rotation = defaultRotation;

        public RotationSettings rotation;

        //[Range(0.5f, 2.5f)]
        //public float scale = defaultScale;

        public ScaleSettings scale;

        //[Range(1, 50)]
        //public int spawnCount = defaultSpawnCount;

        public SpawnCountSettings spawnCount;
    }

    [SerializeField] GameObject[] zones;
    [SerializeField] Settings settings;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] bool clampSpeedToScale = false;

    //[Space(10)]
    //[SerializeField] int exampleOfSpace;

    private class Journey
    {
        public Vector2 Origin { get; set; }
        public Vector2 Target { get; set; }
    }

    private int spawnCount, activeSpawnCount;

    void Awake()
    {
        ResolveComponents();
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(Co_SpawnAsteroids());
    }

    public GameObject[] GetZones()
    {
        return zones;
    }

    public Settings GetSettings()
    {
        return settings;
    }

    public GameObject[] GetPrefabs()
    {
        return prefabs;
    }

    public bool GetClampSpeedToScale()
    {
        return clampSpeedToScale;
    }

    private void ResolveComponents() { }

    private IEnumerator Co_SpawnAsteroids()
    {
        //spawnCount = UnityEngine.Random.Range(settings.minSpawnCount, spawnSsettingsettings.spawnCount + 1);
        spawnCount = UnityEngine.Random.Range(settings.spawnCount.min, settings.spawnCount.max + 1);
        activeSpawnCount = 0;

        while (true)
        {
            if (activeSpawnCount < spawnCount)
            {
                GameObject asteroidPrefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length - 1)];
                //float speed = UnityEngine.Random.Range(Settings.minSpeed, settings.speed);
                float speed = UnityEngine.Random.Range(settings.speed.min, settings.speed.max);
                //float rotation = UnityEngine.Random.Range(Settings.minRotation, settings.rotation);
                float rotation = UnityEngine.Random.Range(settings.rotation.min, settings.rotation.max);
                //float scale = UnityEngine.Random.Range(Settings.minScale, settings.scale);
                float scale = UnityEngine.Random.Range(settings.scale.min, settings.scale.max);

                if (clampSpeedToScale)
                {
                    speed /= scale;
                }

                yield return StartCoroutine(Co_SpawnAsteroid(asteroidPrefab, speed, rotation, scale));
                ++activeSpawnCount;

                //float delay = UnityEngine.Random.Range(Settings.minInterSpawnDelay, settings.interSpawnDelay);
                float delay = UnityEngine.Random.Range(settings.interSpawnDelay.min, settings.interSpawnDelay.max);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator Co_SpawnAsteroid(GameObject asteroidPrefab, float speed, float rotation, float scale)
    {
        Journey journey = CreateJourney();
        Vector2 origin = journey.Origin;
        Vector2 target = journey.Target;

        var asteroid = Instantiate(asteroidPrefab, origin, Quaternion.identity) as GameObject;
        asteroid.transform.parent = transform;
        asteroid.transform.localScale = new Vector3(scale, scale, 1.0f);

        var renderer = asteroid.GetComponent<SpriteRenderer>() as SpriteRenderer;
        renderer.sortingOrder = settings.sortingOrder;

        var asteroidController = asteroid.GetComponent<AsteroidController>() as AsteroidController;

        if (asteroidController != null)
        {
            asteroidController.RegisterDelegates(new AsteroidController.Delegates
            {
                OnAsteroidDamagedDelegate = OnAsteroidDamaged,
                OnAsteroidDestroyedDelegate = OnAsteroidDestroyed,
                OnAsteroidJourneyCompleteDelegate = OnAsteroidJourneyComplete
            });

            asteroidController.Actuate(new AsteroidController.Configuration
            {
                StartTransformTime = Time.time,
                TargetPosition = target,
                Speed = speed,
                Rotation = rotation
            });
        }
        else
        {
            var renderOnlyAsteroidController = asteroid.GetComponent<RenderOnlyAsteroidController>() as RenderOnlyAsteroidController;

            if (renderOnlyAsteroidController != null)
            {
                renderOnlyAsteroidController.RegisterDelegates(new RenderOnlyAsteroidController.Delegates
                {
                    OnAsteroidJourneyCompleteDelegate = OnAsteroidJourneyComplete
                });

                renderOnlyAsteroidController.Actuate(new RenderOnlyAsteroidController.Configuration
                {
                    EnableCollisions = enableCollisions,
                    StartTransformTime = Time.time,
                    TargetPosition = target,
                    Speed = speed,
                    Rotation = rotation
                });
            }
        }
        
        yield return null;
    }

    private Journey CreateJourney()
    {
        BoxCollider2D collider;

        IList<GameObject> availableZones = zones.ToList();

        int originIndex = PickAvailableZone(availableZones);

        collider = availableZones[originIndex].GetComponent<BoxCollider2D>() as BoxCollider2D;
        Vector2 originZoneSize = collider.size;
        Vector2 origin = PickPositionInScope(availableZones[originIndex].transform.position, originZoneSize);

        availableZones.RemoveAt(originIndex);

        int targetIndex = PickAvailableZone(availableZones);
        collider = availableZones[targetIndex].GetComponent<BoxCollider2D>() as BoxCollider2D;
        Vector2 targetZoneSize = collider.size;
        Vector2 target = PickPositionInScope(availableZones[targetIndex].transform.position, targetZoneSize);

        return new Journey
        {
            Origin = origin,
            Target = target
        };
    }

    private bool enableCollisions = true;

    public void EnableCollisions(bool active)
    {
        enableCollisions = !enableCollisions;

        //var asteroidControllers = GameObject.FindObjectsOfType<AsteroidController>() as AsteroidController[];

        //foreach (AsteroidController asteroidController in asteroidControllers)
        //{
        //    Collider2D collider = gameObject.GetComponent<Collider2D>() as Collider2D;

        //    if (collider != null)
        //    {
        //        collider.enabled = enabled;
        //    }

        //    var healthBarCanvas = gameObject.transform.Find("Health Bar Canvas");

        //    if (healthBarCanvas != null)
        //    {
        //        healthBarCanvas.gameObject.SetActive(enabled);
        //    }
        //}

        var iCollisions = gameObject.GetComponentsInChildren<IState>() as IState[];

        if (iCollisions != null)
        {
            foreach (IState iCollision in iCollisions)
            {
                iCollision.SetActive(active);
            }
        }
    }

    private void HandleAsteroidEndOfLife()
    {
        --activeSpawnCount;
        //spawnCount = UnityEngine.Random.Range(settings.minSpawnCount, settings.spawnCount + 1);
        spawnCount = UnityEngine.Random.Range(settings.spawnCount.min, settings.spawnCount.max + 1);
    }

    public void OnAsteroidDamaged(GameObject gameObject, GameObject trigger, HealthAttributes healthAttributes) { }
    
    public void OnAsteroidDestroyed(GameObject gameObject, GameObject trigger)
    {
        HandleAsteroidEndOfLife();
    }

    public void OnAsteroidJourneyComplete(GameObject gameObject)
    {
        HandleAsteroidEndOfLife();
    }

    private int PickAvailableZone(IList<GameObject> zones)
    {
        return UnityEngine.Random.Range(0, zones.Count);
    }

    private Vector2 PickPositionInScope(Vector2 position, Vector2 scope)
    {
        return new Vector2
        {
            x = UnityEngine.Random.Range(position.x - (scope.x / 2), position.x + (scope.x / 2)),
            y = UnityEngine.Random.Range(position.y - (scope.y / 2), position.y + (scope.y / 2))
        };
    }
}