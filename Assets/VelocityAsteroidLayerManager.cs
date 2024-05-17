using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class VelocityAsteroidLayerManager : MonoBehaviour
{
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
        public InterSpawnDelaySettings interSpawnDelay;
        public SpeedSettings speed;
        public RotationSettings rotation;
        public ScaleSettings scale;
        public SpawnCountSettings spawnCount;
    }

    [SerializeField] GameObject[] zones;
    [SerializeField] Settings settings;
    [SerializeField] GameObject[] prefabs;
    [SerializeField] bool clampSpeedToScale = false;

    private class Journey
    {
        public Vector2 Origin { get; set; }
        public Vector2 Vector { get; set; }
        public string Zone { get; set; }
    }

    private int spawnCount, activeSpawnCount;

    void Awake()
    {
        ResolveComponents();
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(SpawnAsteroids());
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

    private IEnumerator SpawnAsteroids()
    {
        spawnCount = UnityEngine.Random.Range(settings.spawnCount.min, settings.spawnCount.max + 1);
        activeSpawnCount = 0;

        while (true)
        {
            if (activeSpawnCount < spawnCount)
            {
                GameObject asteroidPrefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length - 1)];
                float speed = UnityEngine.Random.Range(settings.speed.min, settings.speed.max);
                float rotation = UnityEngine.Random.Range(settings.rotation.min, settings.rotation.max);
                float scale = UnityEngine.Random.Range(settings.scale.min, settings.scale.max);

                if (clampSpeedToScale)
                {
                    speed /= scale;
                }

                yield return StartCoroutine(SpawnAsteroid(asteroidPrefab, speed, rotation, scale));
                ++activeSpawnCount;

                float delay = UnityEngine.Random.Range(settings.interSpawnDelay.min, settings.interSpawnDelay.max);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator SpawnAsteroid(GameObject asteroidPrefab, float speed, float rotation, float scale)
    {
        Journey journey = CreateJourney();
        Vector2 origin = journey.Origin;
        Vector2 vector = journey.Vector;
        string zone = journey.Zone;

        var asteroid = Instantiate(asteroidPrefab, origin, Quaternion.identity) as GameObject;
        asteroid.transform.parent = transform;
        asteroid.transform.localScale = new Vector3(scale, scale, 1.0f);

        var renderer = asteroid.GetComponent<SpriteRenderer>() as SpriteRenderer;
        renderer.sortingOrder = settings.sortingOrder;

        var asteroidController = asteroid.GetComponent<VelocityAsteroidController>() as VelocityAsteroidController;

        if (asteroidController != null)
        {
            asteroidController.RegisterDelegates(new VelocityAsteroidController.Delegates
            {
                OnAsteroidDamagedDelegate = OnAsteroidDamaged,
                OnAsteroidDestroyedDelegate = OnAsteroidDestroyed,
                OnAsteroidJourneyCompleteDelegate = OnAsteroidJourneyComplete
            });

            asteroidController.Actuate(new VelocityAsteroidController.Configuration
            {
                //Layer = RenderLayer.SUB_SURFACE,
                StartTransformTime = Time.time,
                Vector = vector,
                Zone = zone,
                Speed = speed,
                Rotation = rotation
            });

            //Debug.Log($"Signature: {asteroidController.Signature} Vector: {vector} Zone: {zone} Speed: {speed} Rotation: {rotation}");
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

        //Debug.Log($"Origin Index: {originIndex} Zone: {availableZones[originIndex].name}");

        availableZones.RemoveAt(originIndex);

        int targetIndex = PickAvailableZone(availableZones);
        collider = availableZones[targetIndex].GetComponent<BoxCollider2D>() as BoxCollider2D;
        Vector2 targetZoneSize = collider.size;
        Vector2 target = PickPositionInScope(availableZones[targetIndex].transform.position, targetZoneSize);
        Vector2 vector = (target - origin).normalized;

        //Debug.Log($"Target Index: {targetIndex} Zone: {availableZones[targetIndex].name}");

        return new Journey
        {
            Origin = origin,
            Zone = availableZones[targetIndex].name,
            Vector = vector
        };
    }

    private bool enableCollisions = true;

    public void EnableCollisions(bool active)
    {
        enableCollisions = !enableCollisions;

        var iStates = gameObject.GetComponentsInChildren<IState>() as IState[];

        if (iStates != null)
        {
            foreach (IState iState in iStates)
            {
                iState.SetActive(active);
            }
        }
    }

    private void HandleAsteroidEndOfLife()
    {
        --activeSpawnCount;
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