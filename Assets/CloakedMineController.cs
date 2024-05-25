using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
public class CloakedMineController : MonoBehaviour
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
    [SerializeField] float turnSpeed = 1.0f;

    [Header("Health")]
    [SerializeField] HealthBarSliderUIManager healthBarSliderUIManager;

    [Header("Explosions")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosiveAudio;

    private Delegates delegates;
    private HealthAttributes healthAttributes;

    void Awake()
    {
        ResolveComponents();
    }

    IEnumerator Start()
    {
        healthBarSliderUIManager?.SetMaxHealth(healthAttributes.GetHealthMetric());
        
        while (true)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0.0f, 0.0f, 1.0f), turnSpeed * 10.0f * Time.deltaTime); ;
            yield return null;
        }
    }

    private void ResolveComponents()
    {
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            GameObject trigger = collider.gameObject;

            var damageAttributes = trigger.GetComponent<DamageAttributes>() as DamageAttributes;
            float damageMetric = damageAttributes.GetDamageMetric();
            healthAttributes.SubstractHealth(damageMetric);
            healthBarSliderUIManager.SetHealth(healthAttributes.GetHealthMetric());

            if (healthAttributes.GetHealthMetric() > 0.0f)
            {
                StartCoroutine(Co_ManifestDamage());
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
                    Range = 2.5f,
                    Speed = 0.5f,
                    DamageMetric = mineDamageAttributes.GetDamageMetric()
                });

                Destroy(explosion, 0.15f);

                AudioSource.PlayClipAtPoint(explosiveAudio, Camera.main.transform.position, 2.0f);

                Destroy(gameObject);

                delegates?.OnMineDestroyedDelegate?.Invoke(gameObject);
            }
        }
    }

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
}