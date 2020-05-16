using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
public class ProximityMineController : BaseMonoBehaviour, IActuate, IState, IModify, INotify
{
    public delegate void OnMineDamaged(GameObject gameObject, HealthAttributes healthAttributes);
    public delegate void OnMineDestroyed(GameObject gameObject);

    public class Delegates
    {
        public OnMineDamaged OnMineDamagedDelegate { get; set; }
        public OnMineDestroyed OnMineDestroyedDelegate { get; set; }
    }

    public class Configuration : GameplayConfiguration { }

    [Header("Mine")]
    [Range(0.1f, 10.0f)]
    [SerializeField] float turnSpeed = 2.5f;
    [SerializeField] Mode mode = Mode.ACTIVE;
    [SerializeField] float activationDelay = 1.0f;

    [Header("Components")]
    [SerializeField] GameObject healthBarCanvas;
    [SerializeField] GameObject countdownCanvas;
    [SerializeField] int startAtCount = 3;

    [Header("Explosions")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosiveAudio;

    private Delegates delegates;
    private SpriteRenderer renderer;
    private HealthBarSliderUIManager healthBarSliderUIManager;
    private TextUIManager textUIManager;
    private HealthAttributes healthAttributes;
    private DamageAttributes damageAttributes;
    private float propagationWaveSpeed = 0.5f;
    private float range;
    private RenderLayer layer;
    private Mode activeMode;
    //private Color originalColor;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();

        range = transform.localScale.x * 2.5f;
        healthBarSliderUIManager?.SetMaxHealth(healthAttributes.GetHealthMetric());
        activeMode = Mode.INACTIVE;
        //originalColor = renderer.color;
    }

    public void SetActive(bool active)
    {
        activeMode = (active) ? Mode.ACTIVE : Mode.INACTIVE;

        healthBarCanvas.GetComponent<Canvas>().enabled = active;
        countdownCanvas.GetComponent<Canvas>().enabled = active;
        //renderer.color = (active) ? originalColor : new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
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
                layer = ((GameplayConfiguration)configuration).Layer;
            }

            if (typeof(Configuration).IsInstanceOfType(configuration)) { }
        }

        StartCoroutine(ActuateCoroutine());
    }

    private IEnumerator ActuateCoroutine()
    {
        gameObject.layer = (int) layer;

        int sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        RenderLayer activeLayer = SetupHelper.SetupManager.GetActiveLayer();
        SetActive((mode == Mode.ACTIVE) && (gameObject.layer == (int) activeLayer));

        StartCoroutine(DetectRadialCollidersCoroutine());

        while (healthAttributes.GetHealthMetric() > 0.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0.0f, 0.0f, 1.0f), turnSpeed * 10.0f * Time.deltaTime); ;
            yield return null;
        }
    }

    private void ResolveComponents()
    {
        renderer = GetComponent<SpriteRenderer>() as SpriteRenderer;
        healthBarSliderUIManager = healthBarCanvas.GetComponentInChildren<HealthBarSliderUIManager>() as HealthBarSliderUIManager;
        textUIManager = countdownCanvas.GetComponentInChildren<TextUIManager>() as TextUIManager;
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
        damageAttributes = GetComponent<DamageAttributes>() as DamageAttributes;
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    private IEnumerator DetectRadialCollidersCoroutine()
    {
        bool autoDestruct = false;

        while (!autoDestruct)
        {
            if (!IsActive())
            {
                yield return null;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(VectorFunctions.ToVector2(transform.position), range);

            if ((colliders != null) && (colliders.Length > 0))
            {
                foreach (Collider2D collider in colliders)
                {
                    GameObject trigger = collider.gameObject;

                    if (trigger != gameObject)
                    {
                        if (trigger.tag.Equals("Boundary"))
                        {
                            continue;
                        }

                        if (trigger.layer != (int) layer)
                        {
                            continue;
                        }

                        autoDestruct = true;
                    }
                }
            }

            yield return null;
        }

        StartCoroutine(DestructObjectCoroutine());

        delegates?.OnMineDestroyedDelegate?.Invoke(gameObject);
    }

    private void DestructObject()
    {
        var explosion = Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        explosion.transform.localScale = transform.localScale;

        var explosionController = explosion.GetComponent<ExplosionController>() as ExplosionController;
        explosionController.Actuate(new ExplosionController.Configuration
        {
            Layer = layer,
            Range = range,
            Speed = propagationWaveSpeed,
            DamageMetric = damageAttributes.GetDamageMetric()
        });

        Destroy(explosion, 0.15f);

        AudioSource.PlayClipAtPoint(explosiveAudio, Camera.main.transform.position, 2.0f);

        Destroy(gameObject);

        delegates?.OnMineDestroyedDelegate?.Invoke(gameObject);
    }

    private IEnumerator DestructObjectCoroutine()
    {
        healthBarCanvas.GetComponent<Canvas>().enabled = false;

        float activationIntervalDelay = activationDelay / startAtCount;

        for (int itr = 0; itr < startAtCount; ++itr)
        {
            textUIManager.SetText((startAtCount - itr).ToString());
            yield return new WaitForSeconds(activationIntervalDelay);
        }

        DestructObject();
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsActive())
        {
            GameObject trigger = collider.gameObject;

            var damageAttributes = trigger.GetComponent<DamageAttributes>() as DamageAttributes;

            if (damageAttributes != null)
            {
                float damageMetric = damageAttributes.GetDamageMetric();
                healthAttributes.SubstractHealth(damageMetric);
                healthBarSliderUIManager?.SetHealth(healthAttributes.GetHealthMetric());

                if (healthAttributes.GetHealthMetric() > 0.0f)
                {
                    StartCoroutine(ManifestDamage());
                    delegates?.OnMineDamagedDelegate?.Invoke(gameObject, healthAttributes);
                }
                else
                {
                    DestructObject();
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

    //public new Defaults GetDefaults()
    //{
    //    return base.GetDefaults();
    //}

    public RenderLayer GetLayer()
    {
        return layer;
    }

    public void SetScale(float multiplier)
    {
        Vector3 defaultScale = GetDefaults().Transform.localScale;
        transform.localScale = defaultScale * multiplier;
    }

    public void OnLayerChange(int layer)
    {
        if ((gameObject.activeSelf) && (mode == Mode.ACTIVE))
        {
            SetActive((mode == Mode.ACTIVE) && (gameObject.layer == layer));
        }
    }
}