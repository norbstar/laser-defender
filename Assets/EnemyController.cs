﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
public class EnemyController : MonoBehaviour, IActuate
{
    public static float ClipDuration = 1.0f;

    public delegate void OnEnemyDamaged(GameObject gameObject, int waveSequence, HealthAttributes healthAttributes);
    public delegate void OnEnemyDestroyed(GameObject gameObject, int waveSequence, bool selfAdministered);

    public class Configuration : GameplayConfiguration
    {
        //public TextPack TextPack { get; set; }
    }

    public class Delegates
    {
        public OnEnemyDamaged OnEnemyDamagedDelegate { get; set; }
        public OnEnemyDestroyed OnEnemyDestroyedDelegate { get; set; }
    }

    [SerializeField] GameObject ship;
    [SerializeField] int pointsAwarded;

    private Animator animator;
    private Delegates delegates;
    private HealthAttributes healthAttributes;
    private GameObject player;

    private float? lastXPosition;
    private float lastXDelta;

    public class PositionVector
    {
        public Vector2 Position { get; set; }
        public Vector2? ProjectedPosition { get; set; }
    }

    public enum AnimationType
    {
        CURVE,
        VECTOR
    }

    private float startTime, speed;
    private bool invertX, invertY, enableBuffeting;
    private AnimationClip animationClip;
    private GameObject pathPrefab;
    private IList<Transform> waypoints;
    private AnimationCurve xPositionCurve, yPositionCurve;
    private AnimationType xType, yType;
    private int waveSequence;

    void Awake()
    {
        ResolveComponents();

    }

    public void Actuate(IConfiguration configuration)
    {
        if (typeof(Configuration).IsInstanceOfType(configuration))
        {
            //this.textPack = ((Configuration) configuration).TextPack;
        }

        if (gameObject.activeSelf)
        {
            StartCoroutine(Co_Actuate());
        }
    }

    private void ResolveDependencies() { }

    private IEnumerator Co_Actuate()
    {
        ResolveDependencies();

        float startTime = Time.time;
        bool complete = false;
        float magnitude = 1.0f;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * (speed * magnitude);

            if (fractionComplete >= 0.0f)
            {
                if (lastXPosition.HasValue)
                {
                    lastXDelta = transform.position.x - lastXPosition.Value;

                    if (lastXDelta != 0.0f)
                    {
                        animator?.SetFloat("speed", lastXDelta * 6.5f);
                    }
                }

                lastXPosition = transform.position.x;
                PositionVector positionVector = CalculatePositionPointData(fractionComplete);
                transform.position = VectorFunctions.ToVector3(positionVector.Position, transform.position.z);

                if (positionVector.ProjectedPosition.HasValue)
                {
                    transform.rotation = MathFunctions.AlignZRotationToVector(transform.rotation, positionVector.Position, (Vector2) positionVector.ProjectedPosition, 90);
                }

                complete = fractionComplete >= 1f;
            }

            if (complete)
            {
                OnJourneyComplete(waveSequence);
            }

            yield return null;
        }
    }

    private void ResolveComponents()
    {
        if (ship != null)
        {
            animator = ship.GetComponent<Animator>() as Animator;
        }

        player = GameObject.FindGameObjectWithTag("Player") as GameObject;
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
    }

    public void Configure(WaveConfig waveConfig, int waveSequence)
    {
        animationClip = waveConfig.GetAnimationClip();
        invertX = waveConfig.GetInvertX();
        invertY = waveConfig.GetInvertY();
        enableBuffeting = waveConfig.GetEnableBuffeting();
        speed = waveConfig.GetSpeed();
        pathPrefab = waveConfig.GetPathPrefab();
        waypoints = waveConfig.GetWaypoints();
        this.waveSequence = waveSequence;

        var pathConfig = pathPrefab.GetComponent<PathConfig>() as PathConfig;
        PathConfig.Smoothing smoothing = pathConfig.GetSmoothing();

        xType = (smoothing.xTangents) ? AnimationType.CURVE : AnimationType.VECTOR;
        xPositionCurve = (smoothing.xTangents) ? CreateXCurve(waypoints, ClipDuration, smoothing.xTangents) : null;

        yType = (smoothing.yTangents) ? AnimationType.CURVE : AnimationType.VECTOR;
        yPositionCurve = (smoothing.yTangents) ? CreateYCurve(waypoints, ClipDuration, smoothing.yTangents) : null;
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    private PositionVector CalculatePositionPointData(float fractionComplete)
    {
        Vector2 position = CalculatePosition(fractionComplete);
        Vector2? projectedPosition = null;

        if ((fractionComplete + 0.1f) <= 1.0f)
        {
            projectedPosition = CalculatePosition(fractionComplete + 0.1f);
        }

        if (enableBuffeting)
        {
            Vector2 buffeting = new Vector2
            {
                x = Random.Range(0.0f, 0.25f),
                y = Random.Range(0.0f, 0.25f)
            };

            position += buffeting;

            if (projectedPosition != null)
            {
                projectedPosition += buffeting;
            }
        }

        if (invertX)
        {
            position = new Vector2(InGameManager.ScreenRatio.x - position.x, position.y);

            if (projectedPosition != null)
            {
                projectedPosition = new Vector2(InGameManager.ScreenRatio.x - ((Vector2) projectedPosition).x, ((Vector2) projectedPosition).y);
            }
        }

        if (invertY)
        {
            position = new Vector2(position.x, InGameManager.ScreenRatio.y - position.y);

            if (projectedPosition != null)
            {
                projectedPosition = new Vector2(((Vector2) projectedPosition).x, InGameManager.ScreenRatio.y - ((Vector2) projectedPosition).y);
            }
        }

        return new PositionVector
        {
            Position = position,
            ProjectedPosition = projectedPosition
        };
    }

    private AnimationCurve CreateXCurve(IList<Transform> waypoints, float duration, bool smoothTangents)
    {
        var keyframes = new List<Keyframe>();
        float keyFrameDuration = duration / (waypoints.Count - 1);

        for (int itr = 0; itr < waypoints.Count; ++itr)
        {
            float timestamp = itr * keyFrameDuration;
            keyframes.Add(new Keyframe(timestamp, waypoints[itr].position.x));
        }

        return AnimationFunctions.CreateCustomCurve(keyframes.ToArray(), smoothTangents);
    }

    private AnimationCurve CreateYCurve(IList<Transform> waypoints, float duration, bool smoothTangents)
    {
        var keyframes = new List<Keyframe>();
        float keyFrameDuration = duration / (waypoints.Count - 1);

        for (int itr = 0; itr < waypoints.Count; ++itr)
        {
            float timestamp = itr * keyFrameDuration;
            keyframes.Add(new Keyframe(timestamp, waypoints[itr].position.y));
        }

        return AnimationFunctions.CreateCustomCurve(keyframes.ToArray(), smoothTangents);
    }

    private Vector2 CalculatePosition(float fractionComplete)
    {
        float x = 0.0f, y = 0.0f;

        switch (xType)
        {
            case AnimationType.CURVE:
                x = xPositionCurve.Evaluate(fractionComplete);
                break;

            case AnimationType.VECTOR:
                float waypointDuration = ClipDuration / (waypoints.Count - 1);

                for (int itr = 0; itr < waypoints.Count/* - 1*/; ++itr)
                {
                    if ((fractionComplete >= waypointDuration * itr) && (fractionComplete <= waypointDuration * (itr + 1)))
                    {
                        float origin = waypoints[itr].position.x;
                        float target = waypoints[itr + 1].position.x;
                        x = CalculateAxialPosition(fractionComplete, waypointDuration, itr, origin, target);
                    }
                }
                break;
        }

        switch (yType)
        {
            case AnimationType.CURVE:
                y = yPositionCurve.Evaluate(fractionComplete);
                break;

            case AnimationType.VECTOR:
                float waypointDuration = ClipDuration / (waypoints.Count - 1);

                for (int itr = 0; itr < waypoints.Count - 1; ++itr)
                {
                    if ((fractionComplete >= waypointDuration * itr) && (fractionComplete <= waypointDuration * (itr + 1)))
                    {
                        float origin = waypoints[itr].position.y;
                        float target = waypoints[itr + 1].position.y;
                        y = CalculateAxialPosition(fractionComplete, waypointDuration, itr, origin, target);
                    }
                }
                break;
        }

        return new Vector2(x, y);
    }

    private float CalculateAxialPosition(float fractionComplete, float waypointDuration, int itr, float origin, float target)
    {
        float originTimestamp = (waypointDuration * itr);
        float targetTimestamp = (waypointDuration * (itr + 1));
        float offset = fractionComplete - originTimestamp;
        float interFractionComplete = offset / (targetTimestamp - originTimestamp);
        float delta = target - origin;

        return origin + (delta * interFractionComplete);
    }

    public int GetPointsAwarded()
    {
        return pointsAwarded;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!InBoundsOfCamera(transform.position))
        {
            return;
        }

        if (collider != null)
        {
            GameObject trigger = collider.gameObject;

            if (trigger.tag.Equals("Projectile"))
            {
                Destroy(trigger);
                var damageAttributes = trigger.GetComponent<DamageAttributes>() as DamageAttributes;

                if (damageAttributes != null)
                {
                    float damageMetric = damageAttributes.DamageMetric;
                    healthAttributes.SubstractHealth(damageMetric);
                }

                if (healthAttributes.HealthMetric > 0)
                {
                    delegates?.OnEnemyDamagedDelegate?.Invoke(gameObject, waveSequence, healthAttributes);
                    StartCoroutine(Co_ManifestDamage());
                }
                else
                {
                    OnDestroyed(waveSequence);
                }
            }
        }
    }

    private IEnumerator Co_ManifestDamage()
    {
        for (int itr = 0; itr < 5; ++itr)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private bool InBoundsOfCamera(Vector3 position)
    {
        return (position.x >= 0.0f) && (position.x <= InGameManager.ScreenRatio.x - 1.0f) &&
            (position.y >= 0.0f) && (position.y <= InGameManager.ScreenRatio.y - 1.0f);
    }

    private void OnJourneyComplete(int waveSequence)
    {
        delegates?.OnEnemyDestroyedDelegate?.Invoke(gameObject, waveSequence, true);
    }

    private void OnDestroyed(int waveSequence)
    {
        if (healthAttributes.HealthMetric <= 0.0f)
        {
            delegates.OnEnemyDestroyedDelegate?.Invoke(gameObject, waveSequence, false);
        }
    }
}