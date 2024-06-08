using System.Collections;

using UnityEngine;

public class VelocityAsteroidController : BaseMonoBehaviour, IActuate, IModify, INotify
{
    public delegate void OnAsteroidDamaged(GameObject gameObject, GameObject trigger, HealthAttributes healthAttributes);
    public delegate void OnAsteroidDestroyed(GameObject gameObject, GameObject trigger);
    public delegate void OnAsteroidJourneyComplete(GameObject gameObject);

    public class Delegates
    {
        public OnAsteroidDamaged OnAsteroidDamagedDelegate { get; set; }
        public OnAsteroidDestroyed OnAsteroidDestroyedDelegate { get; set; }
        public OnAsteroidJourneyComplete OnAsteroidJourneyCompleteDelegate { get; set; }
    }

    public class Configuration : GameplayConfiguration
    {
        public float StartTransformTime { get; set; }
        public Vector2 Vector { get; set; }
        public string Zone { get; set; }
        public float Speed { get; set; }
        public float Rotation { get; set; }
    }

    [Header("Asteroid")]
    [SerializeField] Mode mode = Mode.ACTIVE;

    //[Header("Fragmentations")]
    //[SerializeField] GameObject fragmentationPrefab;
    //[SerializeField] AudioClip fragmentationAudio;

    [Header("Partical Effects")]
    [SerializeField] GameObject particalEffectPrefab;
    [SerializeField] AudioClip particalEffectAudio;

    [Header("Components")]
    [SerializeField] GameObject healthBarCanvas;

    private Delegates delegates;
    private HealthBarSliderUIManager healthBarSliderUIManager;
    private Rigidbody2D rigidBody;
    private HealthAttributes healthAttributes;
    private float startTime;
    private Vector2 vector;
    private string zone;
    private float speed;
    private float rotation;
    private RenderLayer layer;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();

        healthBarSliderUIManager?.SetMaxHealth(healthAttributes.GetHealthMetric());
    }

    public void Actuate(IConfiguration configuration)
    {
        if (configuration != null)
        {
            if (typeof(GameplayConfiguration).IsInstanceOfType(configuration))
            {
                layer = ((GameplayConfiguration) configuration).Layer;
            }

            if (typeof(Configuration).IsInstanceOfType(configuration))
            {
                startTime = ((Configuration) configuration).StartTransformTime;
                vector = ((Configuration) configuration).Vector;
                zone = ((Configuration) configuration).Zone;
                speed = ((Configuration) configuration).Speed;
                rotation = ((Configuration) configuration).Rotation;
            }
        }

        StartCoroutine(Co_Actuate(startTime, vector, speed, rotation));
    }

    private IEnumerator Co_Actuate(float startTime, Vector2 vector, float speed, float rotation)
    {
        gameObject.layer = (int) layer;

        var sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        rigidBody.velocity = vector * speed;
        rigidBody.angularVelocity = rotation * speed;

        yield return null;
    }

    private void ResolveComponents()
    {
        healthBarSliderUIManager = healthBarCanvas.GetComponentInChildren<HealthBarSliderUIManager>() as HealthBarSliderUIManager;
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
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

    public void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject trigger = collider.gameObject;

        //Debug.Log($"Signature: {Signature} Trigger Name: {trigger.name} Tag: {trigger.tag}");

        if (trigger.tag.Equals("Asteroid Layer Boundary") && trigger.name.Equals(zone))
        {
            OnJourneyComplete();
        }
        else if (!trigger.tag.Equals("Asteroid"))
        {
            var damageAttributes = trigger.GetComponent<DamageAttributes>() as DamageAttributes;

            if (damageAttributes != null)
            {
                float damageMetric = damageAttributes.GetDamageMetric();
                healthAttributes.SubstractHealth(damageMetric);
                healthBarSliderUIManager?.SetHealth(healthAttributes.GetHealthMetric());

                if (healthAttributes.GetHealthMetric() > 0.0f)
                {
                    StartCoroutine(Co_ManifestDamage());
                    delegates?.OnAsteroidDamagedDelegate?.Invoke(gameObject, trigger, healthAttributes);
                }
                else
                {
                    var localDamageAttributes = GetComponent<DamageAttributes>() as DamageAttributes;

# if (false)
                    var fragmentation = Instantiate(fragmentationPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                    fragmentation.transform.localScale = transform.localScale;

                    var fragmentationController = fragmentation.GetComponent<FragmentationController>() as FragmentationController;

                    //float intersectionAngle = MathFunctions.GetForwardIntersectionAngle(trigger.transform, transform);
                    float angle = MathFunctions.GetIntersectionAngle(trigger.transform);
                    angle = MathFunctions.ModifyTrueAngle(angle, 90.0f);

                    //float leftOffset = MathFunctions.ModifyTrueAngle(angle, -90.0f);
                    //float rightOffset = MathFunctions.ModifyTrueAngle(angle, +90.0f);
                    //Debug.Log($"Angle: {intersectionAngle} Left Offset: {leftOffset} Right Offset: {rightOffset}");

                    //var vector = (Vector2.right - VectorFunctions.ToVector2(transform.right)).normalized;
                    //var vector = (VectorFunctions.ToVector2(transform.position) - VectorFunctions.ToVector2(trigger.transform.position)).normalized;
                    //Debug.Log($"Vector: {vector}");

                    //trigger.transform.right = new Vector3(0, 1, 0);
                    //Debug.Log($"Trigger Vector: {trigger.transform.right}");

                    //Vector2 vector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, angle) * Vector2.right);
                    //Debug.Log($"Trigger Vector: {vector}");


                    fragmentationController.Actuate(new FragmentationController.Configuration
                    {
                        RefTransform = transform,
                        IntersectionAngle = angle,
                        Prefab = fragmentationPrefab
                    });

                    Destroy(fragmentation, 0.15f);

                    AudioSource.PlayClipAtPoint(fragmentationAudio, Camera.main.transform.position, 2.0f);
#endif 

                    var particalEffect = Instantiate(particalEffectPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                    particalEffect.transform.localScale = transform.localScale;

                    var particalEffectController = particalEffect.GetComponent<ParticalEffectController>() as ParticalEffectController;

                    particalEffectController.Actuate();

                    Destroy(particalEffect, 0.15f);

                    AudioSource.PlayClipAtPoint(particalEffectAudio, Camera.main.transform.position, 2.0f);

                    Destroy(gameObject);

                    delegates?.OnAsteroidDestroyedDelegate?.Invoke(gameObject, trigger);
                }
            }
        }
    }

    //public void OnTriggerExit2D(Collider2D collider)
    //{
    //    if (collider != null)
    //    {
    //        GameObject trigger = collider.gameObject;

    //        if (trigger.tag.Equals("Boundary"))
    //        {
    //            OnJourneyComplete();
    //        }
    //    }
    //}

    private IEnumerator Co_ManifestDamage()
    {
        for (int itr = 0; itr < 3; ++itr)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);

            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
    }

    //public new Defaults GetDefaults()
    //{
    //    return base.GetDefaults();
    //}

    public RenderLayer GetLayer()
    {
        return layer;
    }

    private void OnJourneyComplete()
    {
        Destroy(gameObject);
        delegates?.OnAsteroidJourneyCompleteDelegate?.Invoke(gameObject);
    }
}