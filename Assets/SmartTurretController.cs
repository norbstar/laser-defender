using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
[RequireComponent(typeof(DamageAttributes))]
public class SmartTurretController : BaseMonoBehaviour, IActuate, IModify, INotify
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

    public class Configuration : GameplayConfiguration { }

    [Header("Turret")]
    [Range(0.0f, 50.0f)]
    [SerializeField] float turnSpeed = 10.0f;
    [SerializeField] GameObject mount;
    [SerializeField] GameObject gun;
    [SerializeField] Type type;
    //[SerializeField] Mode mode;
    [SerializeField] Group viableTargets;

    [Header("Components")]
    [SerializeField] GameObject healthBarCanvas;

    [Header("Explosions")]
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] AudioClip explosiveAudio;

    [Header("Projectiles")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] long projectileDelayMs = 250;

    private Delegates delegates;
    private HealthBarSliderUIManager healthBarSliderUIManager;
    private HealthAttributes healthAttributes;
    private RenderLayer layer;
    //private Mode activeMode;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();

        healthBarSliderUIManager?.SetMaxHealth(healthAttributes.GetHealthMetric());
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

            if (typeof(Configuration).IsInstanceOfType(configuration)) { }
        }

        StartCoroutine(ActuateCoroutine());
    }

    private IEnumerator ActuateCoroutine()
    {
        gameObject.layer = (int) layer;

        int sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);
        
        long targetTicks = 0;

        while (healthAttributes.GetHealthMetric() > 0.0f)
        {
            SortedList<float, GameObject> viableTargets = IdentifyViableTargets();
            GameObject target = null;

            foreach (KeyValuePair<float, GameObject> viableTarget in viableTargets)
            {
                GameObject targetGameObject = viableTarget.Value;

                if (targetGameObject.tag.Equals("Projectile"))
                {
                    if (!HasIntersectionTrajectory(targetGameObject))
                    {
                        continue;
                    }
                }

                float angle = MathFunctions.TrueAngle(Mathf.Round(MathFunctions.GetAngle(transform.position, targetGameObject.transform.position, 0)));
                Vector2 direction = MathFunctions.AngleToVector(angle);
                Debug.DrawRay(transform.position, direction, Color.yellow);

                if (HasLineOfSight(targetGameObject, direction))
                {
                    target = targetGameObject;
                    break;
                }
            }

            if (target != null)
            {
                bool success = false;
                yield return StartCoroutine(LockOnTargetCoroutine(target, value => success = value));

                if (success)
                {
                    long ticks = DateTime.Now.Ticks;

                    if (ticks >= targetTicks)
                    {
                        SpawnAndActuateProjectiles();
                        targetTicks = ticks + (projectileDelayMs * TimeSpan.TicksPerMillisecond);
                    }
                }
            }

            yield return null;
        }
    }

    private void ResolveComponents()
    {
        healthBarSliderUIManager = healthBarCanvas.GetComponentInChildren<HealthBarSliderUIManager>() as HealthBarSliderUIManager;
        healthAttributes = GetComponent<HealthAttributes>();
    }

    private class DuplicateKeyComparer<T> : IComparer<T> where T : IComparable
    {
        #region IComparer<T> Members

        public int Compare(T keyA, T keyB)
        {
            return (keyA.CompareTo(keyB) == 0) ? 1 : keyA.CompareTo(keyB);
        }

        #endregion
    }

    private SortedList<float, GameObject> IdentifyViableTargets()
    {
        var targets = new SortedList<float, GameObject>(new DuplicateKeyComparer<float>());

        Collider2D[] colliders = Physics2D.OverlapCircleAll(VectorFunctions.ToVector2(transform.position), Mathf.Infinity);

        if ((colliders != null) && (colliders.Length > 0))
        {
            foreach (Collider2D collider in colliders)
            {
                GameObject trigger = collider.gameObject;

                if ((viableTargets.items.Contains(trigger.tag)) && (!trigger.name.Contains(Signature)) && (trigger.layer == (int) layer))
                {
                    float distance = (trigger.transform.position - transform.position).magnitude;
                    targets.Add(distance, trigger);
                }
            }
        }

        return targets;
    }

    private bool HasIntersectionTrajectory(GameObject gameObject)
    {
        RaycastHit2D[] circleHits = Physics2D.CircleCastAll(gameObject.transform.position, 0.1f, gameObject.transform.up, Mathf.Infinity);

        foreach (RaycastHit2D hit in circleHits)
        {
            if (hit.collider.gameObject == transform.gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasLineOfSight(GameObject gameObject, Vector2 direction)
    {
        RaycastHit2D[] circleHits;
        float radius = (type == Type.SINGLE) ? 0.1f : 0.25f;
        circleHits = Physics2D.CircleCastAll(transform.position, radius, direction, Mathf.Infinity);

        foreach (RaycastHit2D hit in circleHits)
        {
            if (hit.transform.gameObject == transform.gameObject)
            {
                continue;
            }

            if (hit.transform.tag.Equals("Projectile") && hit.transform.name.Contains(Signature))
            {
                continue;
            }

            return (hit.transform.gameObject == gameObject) ? true : false;
        }

        return true;
    }

    private IEnumerator LockOnTargetCoroutine(GameObject target, Action<bool> success)
    {
        bool complete = false;

        float localAngle = (float) Math.Round(transform.rotation.eulerAngles.z);
        float relativeAngle = MathFunctions.TrueAngle((float) Math.Round(MathFunctions.GetAngle(transform.position, target.transform.position, -90)));
        float differentialAngle = relativeAngle - localAngle;
        float targetRotation = localAngle + differentialAngle;

        while (!complete)
        {
            if (target == null)
            {
                success(false);
                break;
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0.0f, 0.0f, targetRotation), Time.deltaTime * turnSpeed * 45.0f);
            complete = ((float)Math.Round(transform.rotation.eulerAngles.z) == targetRotation);

            yield return null;
        };

        success(true);
    }

    private void SpawnAndActuateProjectiles()
    {
        switch (type)
        {
            case SmartTurretController.Type.SINGLE:
                SpawnAndActuateSingleProjectile();
                break;

            case SmartTurretController.Type.DUAL:
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
            float damageMetric = damageAttributes.GetDamageMetric();
            healthAttributes.SubstractHealth(damageMetric);
            healthBarSliderUIManager.SetHealth(healthAttributes.GetHealthMetric());

            if (healthAttributes.GetHealthMetric() > 0.0f)
            {
                StartCoroutine(ManifestDamage());
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
                    DamageMetric = mineDamageAttributes.GetDamageMetric()
                });

                Destroy(explosion, 0.15f);

                AudioSource.PlayClipAtPoint(explosiveAudio, Camera.main.transform.position, 2.0f);

                Destroy(gameObject);

                delegates?.OnTurretDestroyedDelegate?.Invoke(gameObject);
            }
        }
    }

    private IEnumerator ManifestDamage()
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

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;
    //    float radius = (type == Type.SINGLE) ? 0.1f : 0.25f;
    //    Gizmos.DrawWireSphere(transform.position,radius);
    //}
}