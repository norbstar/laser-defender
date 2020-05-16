using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
public class AsteroidController : MonoBehaviour, IActuate, IState, INotify
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
        public Vector2 TargetPosition { get; set; }
        public float Speed { get; set; }
        public float Rotation { get; set; }
    }

    [Header("Asteroid")]
    [SerializeField] Mode mode = Mode.ACTIVE;

    [Header("Components")]
    [SerializeField] GameObject healthBarCanvas;

    //[Header("Explosions")]
    //[SerializeField] GameObject explosionPrefab;
    //[SerializeField] AudioClip explosiveAudio;

    [Header("Fragmentations")]
    [SerializeField] GameObject fragmentationPrefab;
    [SerializeField] AudioClip fragmentationAudio;

    private Delegates delegates;
    private HealthBarSliderUIManager healthBarSliderUIManager;
    private HealthAttributes healthAttributes;
    //private float propagationWaveSpeed = 0.5f;
    //private float range;
    //private bool enableCollisions;
    private float startTime;
    private Vector2 targetPosition;
    private float speed;
    private float rotation;
    private RenderLayer layer;
    private Mode activeMode;

    void Awake()
    {
        ResolveComponents();

        //range = transform.localScale.x * 2.5f;
        healthBarSliderUIManager?.SetMaxHealth(healthAttributes.GetHealthMetric());
        activeMode = Mode.INACTIVE;
    }

    public void SetActive(bool active)
    {
        activeMode = (active) ? Mode.ACTIVE : Mode.INACTIVE;
        healthBarCanvas.GetComponent<Canvas>().enabled = active;
    }

    public bool IsActive()
    {
        return (activeMode == Mode.ACTIVE);
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
                targetPosition = ((Configuration) configuration).TargetPosition;
                speed = ((Configuration) configuration).Speed;
                rotation = ((Configuration) configuration).Rotation;
            }
        }

        StartCoroutine(ActuateCoroutine(startTime, targetPosition, speed, rotation));
    }

    private IEnumerator ActuateCoroutine(float startTime, Vector2 targetPosition, float speed, float rotation)
    {
        gameObject.layer = (int) layer;

        int sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        RenderLayer activeLayer = SetupHelper.SetupManager.GetActiveLayer();
        SetActive((mode == Mode.ACTIVE) && (gameObject.layer == (int)activeLayer));

        Vector2 originPosition = VectorFunctions.ToVector2(transform.position);
        float magnitude = (targetPosition - originPosition).magnitude * 0.01f;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) * (speed * magnitude);

            if (fractionComplete >= 0.0f)
            {
                Vector2 position = CalculatePosition(originPosition, targetPosition, fractionComplete);
                transform.localPosition = VectorFunctions.ToVector3(position, 0.0f);
                transform.localRotation = CalculateRotation(rotation);

                complete = (fractionComplete >= 1.0f);
            }

            if (complete)
            {
                OnJourneyComplete();
            }
        
            yield return null;
        }
    }

    private void ResolveComponents()
    {
        healthBarSliderUIManager = healthBarCanvas.GetComponentInChildren<HealthBarSliderUIManager>() as HealthBarSliderUIManager;
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
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
        if (IsActive())
        {
            GameObject trigger = collider.gameObject;

            if (!trigger.tag.Equals("Asteroid"))
            {
                var damageAttributes = trigger.GetComponent<DamageAttributes>() as DamageAttributes;

                if (damageAttributes != null)
                {
                    float damageMetric = damageAttributes.GetDamageMetric();
                    healthAttributes.SubstractHealth(damageMetric);
                    healthBarSliderUIManager?.SetHealth(healthAttributes.GetHealthMetric());

                    if (healthAttributes.GetHealthMetric() > 0.0f)
                    {
                        StartCoroutine(ManifestDamage());
                        delegates?.OnAsteroidDamagedDelegate?.Invoke(gameObject, trigger, healthAttributes);
                    }
                    else
                    {
                        //var explosion = Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                        //explosion.transform.localScale = transform.localScale;

                        //var explosionController = explosion.GetComponent<ExplosionController>() as ExplosionController;
                        var localDamageAttributes = GetComponent<DamageAttributes>() as DamageAttributes;

                        //explosionController.Actuate(new ExplosionController.Configuration
                        //{
                        //    Range = range,
                        //    Speed = propagationWaveSpeed,
                        //    DamageMetric = localDamageAttributes.GetDamageMetric()
                        //});

                        //Destroy(explosion, 0.15f);

                        //AudioSource.PlayClipAtPoint(explosiveAudio, Camera.main.transform.position, 2.0f);

                        var fragmentation = Instantiate(fragmentationPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                        fragmentation.transform.localScale = transform.localScale;

                        var fragmentationController = fragmentation.GetComponent<FragmentationController>() as FragmentationController;
                            
                        float intersectionAngle = MathFunctions.GetForwardIntersectionAngle(trigger.transform, transform);
                        Debug.Log($"Angle: {intersectionAngle}");

                        fragmentationController.Actuate(new FragmentationController.Configuration
                        {
                            Layer = layer,
                            IntersectionAngle = intersectionAngle,
                            Prefab = fragmentationPrefab
                        });

                        Destroy(fragmentation, 0.15f);

                        AudioSource.PlayClipAtPoint(fragmentationAudio, Camera.main.transform.position, 2.0f);

                        Destroy(gameObject);

                        delegates?.OnAsteroidDestroyedDelegate?.Invoke(gameObject, trigger);
                    }
                }
            }
        }
    }

    private IEnumerator ManifestDamage()
    {
        for (int itr = 0; itr < 3; ++itr)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);

            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void OnJourneyComplete()
    {
        Destroy(gameObject);
        delegates?.OnAsteroidJourneyCompleteDelegate?.Invoke(gameObject);
    }

    public void OnLayerChange(int layer)
    {
        if ((gameObject.activeSelf) && (mode == Mode.ACTIVE))
        {
            SetActive((mode == Mode.ACTIVE) && (gameObject.layer == layer));
        }
    }
}