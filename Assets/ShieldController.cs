using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
public class ShieldController : MonoBehaviour
{
    public delegate void OnShieldDamaged(HealthAttributes healthAttributes);
    public delegate void OnShieldDestroyed();

    public class Delegates
    {
        public OnShieldDamaged OnShieldDamagedDelegate { get; set; }
        public OnShieldDestroyed OnShieldDestroyedDelegate { get; set; }
    }

    [SerializeField] AudioClip powerUpAudio, powerDownAudio;

    private Delegates delegates;
    private new CircleCollider2D collider;
    private HealthAttributes healthAttributes;
    private Animator animator;

    void Awake()
    {
        ResolveComponents();
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    private void ResolveComponents()
    {
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
        collider = GetComponent<CircleCollider2D>() as CircleCollider2D;
        animator = GetComponent<Animator>() as Animator;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            GameObject trigger = collider.gameObject;

            if (trigger.tag.Equals("Player") || trigger.tag.Equals("Boundary"))
            {
                return;
            }

            var damageAttributes = trigger.GetComponent<DamageAttributes>() as DamageAttributes;

            if (damageAttributes != null)
            {
                float damageMetric = damageAttributes.GetDamageMetric();
                healthAttributes.SubstractHealth(damageMetric);
            }

            if (healthAttributes.GetHealthMetric() > 0)
            {
                delegates?.OnShieldDamagedDelegate?.Invoke(healthAttributes);
                StartCoroutine(ManifestDamage());
            }
            else
            {
                Destroy(gameObject);
                AudioSource.PlayClipAtPoint(powerDownAudio, Camera.main.transform.position, 2.0f);

                delegates?.OnShieldDestroyedDelegate?.Invoke();
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
}