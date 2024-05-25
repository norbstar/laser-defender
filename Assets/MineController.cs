using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
public class MineController : BaseMineController, IActuate, IModify, INotify
{
    [Header("Mine")]
    [Range(0.1f, 10.0f)]
    [SerializeField] float turnSpeed = 2.5f;

    [Header("Components")]
    [SerializeField] GameObject healthBarCanvas;

    [Header("Explosions")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosiveAudio;

    protected override IEnumerator Co_Actuate()
    {
        Debug.Log($"{name} Co_Actuate");

        gameObject.layer = (int) layer;

        var sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        while (healthAttributes.GetHealthMetric() > 0.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0.0f, 0.0f, 1.0f), turnSpeed * 10.0f * Time.deltaTime);
            yield return null;
        }
    }

    protected override void ResolveComponents()
    {
        healthBarSliderUIManager = healthBarCanvas.GetComponentInChildren<HealthBarSliderUIManager>();
        healthAttributes = GetComponent<HealthAttributes>();
    }

    private void DestroyObject()
    {
        var explosion = Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        explosion.transform.localScale = transform.localScale;

        var explosionController = explosion.GetComponent<ExplosionController>();
        var mineDamageAttributes = GetComponent<DamageAttributes>();

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
                StartCoroutine(Co_ManifestDamage());
                delegates?.OnMineDamagedDelegate?.Invoke(gameObject, healthAttributes);
            }
            else
            {
                DestroyObject();
            }
        }
    }
}