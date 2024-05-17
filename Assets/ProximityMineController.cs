using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
public class ProximityMineController : BaseMineController, IActuate, IModify, INotify
{
    [Header("Mine")]
    [Range(0.1f, 10.0f)]
    [SerializeField] float turnSpeed = 2.5f;
    [SerializeField] float activationDelay = 1.0f;

    [Header("Components")]
    [SerializeField] GameObject healthBarCanvas;
    [SerializeField] GameObject countdownCanvas;
    [SerializeField] int startAtCount = 3;

    [Header("Explosions")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosiveAudio;

    private TextUIManager textUIManager;
    private DamageAttributes damageAttributes;

    protected override IEnumerator ActuateCoroutine()
    {
        Debug.Log($"{name} ActuateCoroutine");

        gameObject.layer = (int) layer;

        var sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        StartCoroutine(DetectRadialCollidersCoroutine());

        while (healthAttributes.GetHealthMetric() > 0.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0.0f, 0.0f, 1.0f), turnSpeed * 10.0f * Time.deltaTime); ;
            yield return null;
        }
    }

    protected override void ResolveComponents()
    {
        healthBarSliderUIManager = healthBarCanvas.GetComponentInChildren<HealthBarSliderUIManager>();
        textUIManager = countdownCanvas.GetComponentInChildren<TextUIManager>();
        healthAttributes = GetComponent<HealthAttributes>();
        damageAttributes = GetComponent<DamageAttributes>();
    }

    private IEnumerator DetectRadialCollidersCoroutine()
    {
        Debug.Log($"{name} DetectRadialCollidersCoroutine Range: {range}");

        bool autoDestruct = false;

        while (!autoDestruct)
        {
            var colliders = Physics2D.OverlapCircleAll(VectorFunctions.ToVector2(transform.position), range);

            if ((colliders != null) && (colliders.Length > 0))
            {
                foreach (var collider in colliders)
                {
                    var trigger = collider.gameObject;

                    Debug.Log($"{name} DetectRadialCollidersCoroutine Trigger: {trigger.name}");

                    if (trigger != gameObject)
                    {
                        // if (trigger.tag.Equals("Boundary")) continue;

                        // if (trigger.layer != (int) layer) continue;

                        if (trigger.tag.Equals("Player"))
                        {
                            autoDestruct = true;
                        }
                    }
                }
            }

            yield return null;
        }

        StartCoroutine(DestroyObjectCoroutine());
        delegates?.OnMineDestroyedDelegate?.Invoke(gameObject);
    }

    private void DestroyObject()
    {
        Debug.Log($"{name} DestroyObject");

        var explosion = Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        explosion.transform.localScale = transform.localScale;

        var explosionController = explosion.GetComponent<ExplosionController>();
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

    private IEnumerator DestroyObjectCoroutine()
    {
        Debug.Log($"{name} DestroyObjectCoroutine");

        healthBarCanvas.GetComponent<Canvas>().enabled = false;

        var activationIntervalDelay = activationDelay / startAtCount;

        for (int itr = 0; itr < startAtCount; ++itr)
        {
            textUIManager.SetText((startAtCount - itr).ToString());
            yield return new WaitForSeconds(activationIntervalDelay);
        }

        DestroyObject();
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log($"{name} OnTriggerEnter2D");

        var trigger = collider.gameObject;
        var damageAttributes = trigger.GetComponent<DamageAttributes>();

        if (damageAttributes != null)
        {
            var damageMetric = damageAttributes.GetDamageMetric();
            healthAttributes.SubstractHealth(damageMetric);
            healthBarSliderUIManager?.SetHealth(healthAttributes.GetHealthMetric());

            if (healthAttributes.GetHealthMetric() > 0.0f)
            {
                StartCoroutine(ManifestDamage());
                delegates?.OnMineDamagedDelegate?.Invoke(gameObject, healthAttributes);
            }
            else
            {
                DestroyObject();
            }
        }
    }
}