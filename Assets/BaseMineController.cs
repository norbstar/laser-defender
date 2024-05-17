using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
public abstract class BaseMineController : BaseMonoBehaviour, IActuate, IModify, INotify
{
    public delegate void OnMineDamaged(GameObject gameObject, HealthAttributes healthAttributes);
    public delegate void OnMineDestroyed(GameObject gameObject);

    public class Delegates
    {
        public OnMineDamaged OnMineDamagedDelegate { get; set; }
        public OnMineDestroyed OnMineDestroyedDelegate { get; set; }
    }

    public class Configuration : GameplayConfiguration { }

    protected Delegates delegates;
    protected HealthBarSliderUIManager healthBarSliderUIManager;
    protected HealthAttributes healthAttributes;
    protected float propagationWaveSpeed = 0.5f;
    protected float range;
    protected RenderLayer layer;

    public override void Awake()
    {
        base.Awake();
        ResolveComponents();

        range = transform.localScale.x * 2.5f;
        healthBarSliderUIManager?.SetMaxHealth(healthAttributes.GetHealthMetric());
    }

    public void Actuate(IConfiguration configuration)
    {
        Debug.Log($"{name} Actuate");
        
        if (configuration != null)
        {
            if (typeof(GameplayConfiguration).IsInstanceOfType(configuration))
            {
                layer = ((GameplayConfiguration) configuration).Layer;
            }
        }

        StartCoroutine(ActuateCoroutine());
    }

    protected abstract IEnumerator ActuateCoroutine();

    protected abstract void ResolveComponents();

    public void RegisterDelegates(Delegates delegates) => this.delegates = delegates;

    protected abstract void OnTriggerEnter2D(Collider2D collider);

    protected IEnumerator ManifestDamage()
    {
        for (int itr = 0; itr < 3; ++itr)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);

            GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public new Defaults GetDefaults() => base.GetDefaults();

    public RenderLayer GetLayer() => layer;
}