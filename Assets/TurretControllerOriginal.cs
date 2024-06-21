using System;
using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
public class TurretControllerOriginal : BaseMonoBehaviour, IActuate, IModify, INotify
{
    public delegate void OnTurretDamaged(GameObject gameObject, HealthAttributes healthAttributes);
    public delegate void OnTurretDestroyed(GameObject gameObject);

    public enum Type
    {
        SINGLE,
        DUAL
    }

    public class Delegates
    {
        public OnTurretDamaged OnTurretDamagedDelegate { get; set; }
        public OnTurretDestroyed OnTurretDestroyedDelegate { get; set; }
    }

    public class Configuration : GameplayConfiguration
    {
        public GameObject Target { get; set; }
    }

    [Header("Turret")]
    [SerializeField] GameObject mount;
    [SerializeField] GameObject gun;
    [SerializeField] Type type;
    //[SerializeField] Mode mode;
    [SerializeField] GameObject target;
    
    [Range(0.0f, 50.0f)]
    [SerializeField] float turnSpeed = 10.0f;

    [Header("Explosions")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosiveAudio;

    [Header("Projectiles")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] long projectileDelayMs = 250;

    [Header("Health")]
    [SerializeField] HealthBarSliderUIManager healthBarSliderUIManager;

    private Delegates delegates;
    private HealthAttributes healthAttributes;
    private float localAngle, relativeAngle, differentialAngle;
    private long targetTicks;
    private RenderLayer layer;
    //private Mode activeMode;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();

        //mode = activeMode = Mode.INACTIVE;
        //mode = Mode.INACTIVE;
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
                target = ((Configuration) configuration).Target;
            }
        }

        StartCoroutine(Co_Actuate());
    }

    private IEnumerator Co_Actuate()
    {
        gameObject.layer = (int) layer;

        var sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        while (healthAttributes.HealthMetric > 0.0f)
        {
            localAngle = (float) Math.Round(transform.rotation.eulerAngles.z);
            relativeAngle = MathFunctions.TrueAngle((float) Math.Round(MathFunctions.GetAngle(transform.position, target.transform.position, -90)));
            differentialAngle = relativeAngle - localAngle;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0.0f, 0.0f, localAngle + differentialAngle), Time.deltaTime * turnSpeed * 45.0f);

            if (localAngle == localAngle + differentialAngle)
            {
                var up = transform.TransformDirection(Vector2.up);
                Debug.DrawRay(transform.position, up, Color.yellow);

                bool canFire = true;

                RaycastHit2D[] circleHits;
                circleHits = Physics2D.CircleCastAll(transform.position, 0.1f, up, Mathf.Infinity/*, layerMask*/);

                foreach (RaycastHit2D hit in circleHits)
                {
                    if ((hit.transform.gameObject != transform.gameObject) && (hit.collider != null))
                    {
                        if (!hit.transform.gameObject.tag.Equals("Player"))
                        {
                            canFire = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (canFire)
                {
                    StartCoroutine(Co_FireProjectiles(localAngle + differentialAngle));
                }
            }

            yield return null;
        }
    }

    private IEnumerator Co_FireProjectiles(float targetAngle)
    {
        bool firing = true;

        while (firing)
        {
            long ticks = DateTime.Now.Ticks;

            if (ticks >= targetTicks)
            {
                SpawnAndActuateProjectiles();
                targetTicks = ticks + (projectileDelayMs * TimeSpan.TicksPerMillisecond);
            }
            
            firing = (transform.rotation.eulerAngles.z == targetAngle);
            yield return null;
        }
    }

    private void SpawnAndActuateProjectiles()
    {
        switch (type)
        {
            case Type.SINGLE:
                SpawnAndActuateSingleProjectile();
                break;

            case Type.DUAL:
                SpawnAndActuateDualProjectiles();
                break;
        }
    }

    private void SpawnAndActuateSingleProjectile()
    {
        Vector2 position = MathFunctions.GetPosition(VectorFunctions.ToVector2(transform.position), transform.localScale.x * 0.5f, MathFunctions.RelativisticAngle(transform.rotation.eulerAngles.z), 90);
        var projectile = Instantiate(projectilePrefab, new Vector3(position.x, position.y, projectilePrefab.transform.position.z), transform.rotation) as GameObject;
        projectile.name = $"{projectilePrefab.name}-{Signature}";

        var velocityProjectileController = projectile.GetComponent<VelocityProjectileController>() as VelocityProjectileController;

        velocityProjectileController.Actuate(new VelocityProjectileController.Configuration
        {
            Direction = transform.up
        });
    }

    private void SpawnAndActuateDualProjectiles()
    {
        Vector2 position;
        GameObject projectile;
        VelocityProjectileController velocityProjectileController;

        position = MathFunctions.GetPosition(VectorFunctions.ToVector2(transform.position), transform.localScale.x * 0.5f, MathFunctions.RelativisticAngle(transform.rotation.eulerAngles.z), 79.0f);
        projectile = Instantiate(projectilePrefab, new Vector3(position.x, position.y, projectilePrefab.transform.position.z), transform.rotation) as GameObject;
        projectile.name = $"{projectilePrefab.name}-{Signature}";

        velocityProjectileController = projectile.GetComponent<VelocityProjectileController>() as VelocityProjectileController;

        velocityProjectileController.Actuate(new VelocityProjectileController.Configuration
        {
            Direction = transform.up
        });

        position = MathFunctions.GetPosition(VectorFunctions.ToVector2(transform.position), transform.localScale.x * 0.5f, MathFunctions.RelativisticAngle(transform.rotation.eulerAngles.z), 101.0f);
        projectile = Instantiate(projectilePrefab, new Vector3(position.x, position.y, projectilePrefab.transform.position.z), transform.rotation) as GameObject;
        projectile.name = $"{projectilePrefab.name}-{Signature}";

        velocityProjectileController = projectile.GetComponent<VelocityProjectileController>() as VelocityProjectileController;

        velocityProjectileController.Actuate(new VelocityProjectileController.Configuration
        {
            Direction = transform.up
        });
    }

    private void ResolveComponents()
    {
        healthAttributes = GetComponent<HealthAttributes>();
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!InBoundsOfCamera(transform.position))
        {
            return;
        }

        if (collider != null)
        {
            GameObject trigger = collider.gameObject;

            if (trigger.tag.Equals("Projectile"))
            {
                if (trigger.name.Contains(Signature))
                {
                    // Ignore as it's signature matches that of the game object instance
                    return;
                }

                Destroy(trigger);
            }

            var damageAttributes = trigger.GetComponent<DamageAttributes>() as DamageAttributes;

            if (damageAttributes != null)
            {
                float damageMetric = damageAttributes.DamageMetric;
                healthAttributes.SubstractHealth(damageMetric);
                healthBarSliderUIManager.SetHealth(healthAttributes.HealthMetric);

                if (healthAttributes.HealthMetric > 0.0f)
                {
                    StartCoroutine(Co_ManifestDamage());
                    delegates?.OnTurretDamagedDelegate?.Invoke(gameObject, healthAttributes);
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
                        Range = 2.5f,
                        Speed = 0.5f,
                        DamageMetric = mineDamageAttributes.DamageMetric
                    });

                    Destroy(explosion, 0.15f);

                    AudioSource.PlayClipAtPoint(explosiveAudio, Camera.main.transform.position, 2.0f);

                    Destroy(gameObject);

                    delegates?.OnTurretDestroyedDelegate?.Invoke(gameObject);
                }
            }
        }
    }

    private IEnumerator Co_ManifestDamage()
    {
        for (int itr = 0; itr < 3; ++itr)
        {
            mount.GetComponent<SpriteRenderer>().enabled = false;
            gun.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.05f);

            mount.GetComponent<SpriteRenderer>().enabled = true;
            gun.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private bool InBoundsOfCamera(Vector3 position)
    {
        return (position.x >= 0.0f) && (position.x <= InGameManager.ScreenRatio.x - 1.0f) &&
            (position.y >= 0.0f) && (position.y <= InGameManager.ScreenRatio.y - 1.0f);
    }

    //public new Defaults GetDefaults()
    //{
    //    return base.GetDefaults();
    //}

    public RenderLayer GetLayer()
    {
        return layer;
    }
}