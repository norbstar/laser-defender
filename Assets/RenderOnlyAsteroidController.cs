using System.Collections;

using UnityEngine;

public class RenderOnlyAsteroidController : MonoBehaviour, IActuate
{
    public delegate void OnAsteroidJourneyComplete(GameObject gameObject);

    public class Delegates
    {
        public OnAsteroidJourneyComplete OnAsteroidJourneyCompleteDelegate { get; set; }
    }

    public class Configuration : GameplayConfiguration
    {
        public bool EnableCollisions { get; set; }
        public float StartTransformTime { get; set; }
        public Vector3 TargetPosition { get; set; }
        public float Speed { get; set; }
        public float Rotation { get; set; }
    }

    private Delegates delegates;
    private float startTime;
    private Vector3 targetPosition;
    private float speed;
    private float rotation;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration)
    {
        if (typeof(Configuration).IsInstanceOfType(configuration))
        {
            startTime = ((Configuration) configuration).StartTransformTime;
            targetPosition = ((Configuration)configuration).TargetPosition;
            speed = ((Configuration)configuration).Speed;
            rotation = ((Configuration)configuration).Rotation;
        }

        StartCoroutine(Co_Actuate(startTime, targetPosition, speed, rotation));
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    private IEnumerator Co_Actuate(float startTime, Vector3 targetPosition, float speed, float rotation)
    {
        Vector3 originPosition = transform.position;
        float magnitude = (targetPosition - originPosition).magnitude * 0.01f;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) * (speed * magnitude);

            if (fractionComplete >= 0.0f)
            {
                var position = CalculatePosition(originPosition, targetPosition, fractionComplete);
                transform.localPosition = VectorFunctions.ToVector3(position, 0.0f);
                transform.localRotation = CalculateRotation(rotation);

                complete = fractionComplete >= 1f;
            }

            if (complete)
            {
                OnJourneyComplete();
            }

            yield return null;
        }
    }

    private Vector2 CalculatePosition(Vector2 originPosition, Vector2 targetPosition, float fractionComplete)
    {
        return new Vector2(
            Mathf.Lerp(originPosition.x, targetPosition.x, fractionComplete),
            Mathf.Lerp(originPosition.y, targetPosition.y, fractionComplete));
    }

    private Quaternion CalculateRotation(float rotation)
    {
        return Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + (rotation * Time.deltaTime));
    }

    private void OnJourneyComplete()
    {
        Destroy(gameObject);
        delegates?.OnAsteroidJourneyCompleteDelegate?.Invoke(gameObject);
    }
}