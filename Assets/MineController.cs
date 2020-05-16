using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
public class MineController : BaseMonoBehaviour, IActuate, IState, IModify, INotify
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

    [Header("Components")]
    [SerializeField] GameObject healthBarCanvas;

    [Header("Explosions")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosiveAudio;

    private Delegates delegates;
    private HealthBarSliderUIManager healthBarSliderUIManager;
    private HealthAttributes healthAttributes;
    private float propagationWaveSpeed = 0.5f;
    private float range;
    private RenderLayer layer;
    private Mode activeMode;
    private Texture originalTexture;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();

        range = transform.localScale.x * 2.5f;
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

        while (healthAttributes.GetHealthMetric() > 0.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0.0f, 0.0f, 1.0f), turnSpeed * 10.0f * Time.deltaTime);
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
                    var explosion = Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
                    explosion.transform.localScale = transform.localScale;

                    var explosionController = explosion.GetComponent<ExplosionController>() as ExplosionController;
                    var mineDamageAttributes = GetComponent<DamageAttributes>() as DamageAttributes;

                    explosionController.Actuate(new ExplosionController.Configuration
                    {
                        Layer = layer,
                        Range = range,
                        Speed = propagationWaveSpeed,
                        DamageMetric = mineDamageAttributes.GetDamageMetric()
                    });

                    Destroy(explosion, 0.15f);

                    AudioSource.PlayClipAtPoint(explosiveAudio, Camera.main.transform.position, 2.0f);

                    Destroy(gameObject);

                    delegates?.OnMineDestroyedDelegate?.Invoke(gameObject);
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