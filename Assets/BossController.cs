using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
[RequireComponent(typeof(KeyframeSequenceActuationManager))]
public class BossController : MonoBehaviour, IActuate
{
    [Header("Boss")]
    [SerializeField] GameObject body;
    [SerializeField] GameObject[] turrets;
    [SerializeField] Vector3 spawnPoint;

    [Header("Health")]
    [SerializeField] HealthBarSliderUIManager healthBarSliderUIManager;

    [Header("Transitions")]
    [SerializeField] Vector3 ingressPoint;
    [SerializeField] float ingressSpeed = 2.5f;

    [Header("Explosions")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosiveAudio;

    public delegate void OnBossDamaged(GameObject gameObject, HealthAttributes healthAttributes);
    public delegate void OnBossDestroyed(GameObject gameObject);

    public class Delegates
    {
        public OnBossDamaged OnBossDamagedDelegate { get; set; }
        public OnBossDestroyed OnBossDestroyedDelegate { get; set; }
    }

    private Delegates delegates;
    private HealthAttributes healthAttributes;
    private KeyframeSequenceActuationManager keyframeSequenceActuationManager;
    private IDictionary<string, HealthAttributes> turretHealthAttributes;
    private int turretCount;

    void Awake()
    {
        ResolveComponents();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthBarSliderUIManager?.SetMaxHealth(healthAttributes.GetHealthMetric());
    }

    //IEnumerator Start()
    //{
    //    RegisterDelegates();

    //    turretCount = turrets.Length;

    //    yield return StartCoroutine(ActuateCoroutine());
    //}

    public void Actuate(IConfiguration configuration = null)
    {
        RegisterDelegates();

        turretCount = turrets.Length;

        StartCoroutine(Co_Actuate());
    }

    private IEnumerator Co_Actuate()
    {
        yield return StartCoroutine(Co_MoveIntoPosition());
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPoint;
    }

    private IEnumerator Co_MoveIntoPosition()
    {
        Vector3 originPosition = transform.position;
        Vector3 targetPosition = transform.position + ingressPoint;
        float magnitude = (targetPosition - originPosition).magnitude * 0.01f;
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * (ingressSpeed * magnitude * 5.0f);

            if (fractionComplete >= 0.0f)
            {
                transform.position = Vector3.Lerp(originPosition, targetPosition, fractionComplete);
                complete = fractionComplete >= 1f;
            }

            if (complete)
            {
                OnMoveIntoPositionComplete();
            }

            yield return null;
        }
    }

    private void OnMoveIntoPositionComplete() { }

    private void ResolveComponents()
    {
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
        keyframeSequenceActuationManager = GetComponent<KeyframeSequenceActuationManager>() as KeyframeSequenceActuationManager;

        turretHealthAttributes = new Dictionary<string, HealthAttributes>();

        foreach (GameObject turret in turrets)
        {
            turretHealthAttributes.Add(turret.name, turret.GetComponent<HealthAttributes>());
        }
    }

    private void RegisterDelegates()
    {
        var onTriggerHandler = body.GetComponent<OnTriggerHandler>() as OnTriggerHandler;
        onTriggerHandler.RegisterDelegate(OnTriggerBody);

        foreach (GameObject turret in turrets)
        {
            TurretControllerOriginal turretController = turret.GetComponent<TurretControllerOriginal>() as TurretControllerOriginal;
            
            turretController.Actuate(new TurretControllerOriginal.Configuration
            {
                Target = GameObject.FindGameObjectWithTag("Player")
            });

            turretController.RegisterDelegates(new TurretControllerOriginal.Delegates
            {
                OnTurretDamagedDelegate = OnTurretDamaged,
                OnTurretDestroyedDelegate = OnTurretDestroyed
            });
        }
    }

    public void OnTriggerBody(GameObject gameObject)
    {
        if (gameObject.tag.Equals("Projectile"))
        {
            Destroy(gameObject);

            var damageAttributes = gameObject.GetComponent<DamageAttributes>() as DamageAttributes;

            if (damageAttributes != null)
            {
                float damageMetric = damageAttributes.GetDamageMetric();
                healthAttributes.SubstractHealth(damageMetric);
                healthBarSliderUIManager.SetHealth(healthAttributes.GetHealthMetric());

                if (healthAttributes.GetHealthMetric() > 0.0f)
                {
                    OnBodyDamaged(gameObject, healthAttributes);
                }
                else
                {
                    OnBodyDestroyed(body);
                }
            }
        }
    }

    public void OnBodyDamaged(GameObject gameObject, HealthAttributes healthAttributes)
    {
        StartCoroutine(Co_ManifestBodyDamage());
    }

    public void OnBodyDestroyed(GameObject gameObject)
    {
        keyframeSequenceActuationManager.Actuate();
        Destroy(this.gameObject, 0.4f);
    }

    public void OnTurretDamaged(GameObject gameObject, HealthAttributes healthAttributes)
    {
        // TODO
    }

    public void OnTurretDestroyed(GameObject gameObject)
    {
        --turretCount;

        Destroy(Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity), 0.15f);
        AudioSource.PlayClipAtPoint(explosiveAudio, Camera.main.transform.position, 2.0f);

        Destroy(gameObject);

        if (turretCount == 0)
        {
            var renderer = body.GetComponent<SpriteRenderer>() as SpriteRenderer;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1.0f);

            var collider = body.GetComponent<BoxCollider2D>() as BoxCollider2D;
            collider.enabled = true;
        }
    }

    private IEnumerator Co_ManifestBodyDamage()
    {
        IList<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

        foreach (Transform childTransform in transform)
        {
            var spriteRenderer = childTransform.GetComponent<SpriteRenderer>() as SpriteRenderer;

            if ((spriteRenderer != null) && (spriteRenderer.gameObject.activeSelf))
            {
                spriteRenderers.Add(spriteRenderer);
            }
        }

        for (int itr = 0; itr < 3; ++itr)
        {
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = false;
            }

            yield return new WaitForSeconds(0.05f);

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = true;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}