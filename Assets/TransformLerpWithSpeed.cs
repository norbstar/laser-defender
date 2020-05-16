using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TransformLerpWithSpeed : AbstractTransformBehaviour, IActuate, IAcutationTest
{
    [SerializeField] float speed = 1.0f;

    public class Event
    {
        public float Duration { get; set; }
    }

    private Event positionEvent, rotationEvent, scaleEvent;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration)
    {
        if(position.enable)
        {
            float delta = (position.target - originPosition).magnitude;
            float duration = delta / speed;

            positionEvent = new Event
            {
                Duration = duration
            };
        }

        if (rotation.enable)
        {
            float delta = (rotation.target - originRotation).magnitude;
            float duration = delta / speed;

            rotationEvent = new Event
            {
                Duration = duration
            };
        }

        if (scale.enable)
        {

            float delta = (scale.target - originScale).magnitude;
            float duration = delta / speed;

            scaleEvent = new Event
            {
                Duration = duration
            };
        }

        StartCoroutine(TransformCoroutine());
    }

    private IEnumerator TransformCoroutine()
    {
        transform.localPosition = originPosition;
        transform.localRotation = Quaternion.Euler(originRotation);
        transform.localScale = originScale;

        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            complete = true;

            if (positionEvent != null)
            {
                if (!TransformPosition(startTime)) { complete = false; }
            }

            if (rotationEvent != null)
            {
                if (!TransformRotation(startTime)) { complete = false; }
            }

            if (scaleEvent != null)
            {
                if (!TransformScale(startTime)) { complete = false; }
            }

            if (complete)
            {
                OnComplete();
            }

            yield return null;
        }
    }

    private bool TransformPosition(float startTime)
    {
        float duration = positionEvent.Duration;
        float fractionComplete = (Time.time - startTime) / duration;

        transform.localPosition = Vector3.Lerp(originPosition, position.target, fractionComplete);

        return (fractionComplete >= 1.0f);
    }

    private bool TransformRotation(float startTime)
    {
        float duration = rotationEvent.Duration;
        float fractionComplete = (Time.time - startTime) / duration;

        transform.localRotation = Quaternion.Slerp(Quaternion.Euler(originRotation), Quaternion.Euler(rotation.target), fractionComplete);

        return (fractionComplete >= 1.0f);
    }

    private bool TransformScale(float startTime)
    {
        float duration = scaleEvent.Duration;
        float fractionComplete = (Time.time - startTime) / duration;

        transform.localScale = Vector2.Lerp(originScale, scale.target, fractionComplete);

        return (fractionComplete >= 1.0f);
    }

    private void OnComplete()
    {
        Debug.Log($"OnComplete");
    }
}