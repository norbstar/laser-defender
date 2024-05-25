﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
public class DynamicPlayerController : BaseMonoBehaviour
{
    public delegate void OnShipEnaged();
    public delegate void OnShipDisengaged();

    public class Delegates
    {
        public OnShipEnaged OnShipEngagedDelegate { get; set; }
        public OnShipDisengaged OnShipDisengagedDelegate { get; set; }
    }

    [Serializable]
    public class Prefabs
    {
        public GameObject lightBullet;
        public GameObject mediumBullet;
        public GameObject heavyBullet;
        public GameObject lightProton;
        public GameObject mediumProton;
        public GameObject heavyProton;
    }

    [SerializeField] GameObject ship;
    [SerializeField] GameObject[] exhausts;
    [SerializeField] GameObject reverseLeftTrust;
    [SerializeField] GameObject reverseRightTrust;
    [SerializeField] Prefabs prefabs;
    [SerializeField] float speed = 10.0f, transitionSpeed = 2.5f;
    [SerializeField] long projectilesDelayMs = 250;

    public class Boundary
    {
        public float XMin { get; set; }
        public float XMax { get; set; }
        public float YMin { get; set; }
        public float YMax { get; set; }

        public override string ToString() => $"({XMin}, {YMin}, {XMax}, {YMax})";
    }

    private class ExhaustComponent
    {
        public DynamicExhaustController ExhaustController { get; set; }
    }

    public enum ProjectileMode
    {
        Single,
        Dual,
        All
    }

    private Animator animator;
    private HealthAttributes healthAttributes;
    private IList<ExhaustComponent> exhaustComponents;
    private Delegates delegates;
    private Boundary boundary;
    private long targetTicks;
    private bool controlsActive = false;
    private Vector3 defaultPlayerPosition;
    private RenderLayer layer;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();
        ConfigureMovementBoundaries();

        layer = RenderLayer.SURFACE;
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultPlayerPosition = transform.position;
        EngageShip();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ControlsEnabled()) return;

        transform.position = CalculateKeyInputPosition();

        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Co_FirePrimaryProjectile());
        }

        if (Input.GetButtonDown("Fire2"))
        {
            StartCoroutine(Co_FireSecondaryProjectile());
        }
    }

    public void RegisterDelegates(Delegates delegates) => this.delegates = delegates;

    public GameObject[] GetExhausts() => exhausts;

    public GameObject GetReverseLeftTrust() => reverseLeftTrust;

    public GameObject GetReverseRightTrust() => reverseRightTrust;

    public void EnableControls() => controlsActive = true;

    public void DisableControls() => controlsActive = false;

    public bool ControlsEnabled() => controlsActive;

    public void EngageShip() => StartCoroutine(Co_EngageShip());

    private IEnumerator Co_EngageShip()
    {
        var originPosition = transform.position;
        var targetPosition = new Vector3(originPosition.x, 1.5f, originPosition.z);
        var magnitude = (targetPosition - originPosition).magnitude * 0.5f;
        var startTime = Time.time;
        var complete = false;

        SetExhaustThrust(1.0f);

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * (transitionSpeed / magnitude);
            transform.position = Vector3.Lerp(originPosition, targetPosition, (float)fractionComplete);

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnShipEngagedComplete();
            }

            yield return null;
        }

        SetExhaustThrust(0.0f);
    }

    public void DisengageShip() => StartCoroutine(Co_DisengageShip());

    private IEnumerator Co_DisengageShip()
    {
        var originPosition = transform.position;
        var targetPosition = new Vector3(originPosition.x, 17, originPosition.z);
        var magnitude = (targetPosition - originPosition).magnitude * 0.5f;
        var startTime = Time.time;
        var complete = false;

        SetExhaustThrust(1.0f);

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * (transitionSpeed / magnitude);
            transform.position = Vector3.Lerp(originPosition, targetPosition, (float)fractionComplete);

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnShipDisengagedComplete();
            }

            yield return null;
        }

        SetExhaustThrust(0.0f);
    }

    private void SetExhaustThrust(float thrust)
    {
        foreach (ExhaustComponent exhaustComponent in exhaustComponents)
        {
            exhaustComponent.ExhaustController.SetThrust(thrust);
        }
    }

    private IEnumerator Co_FirePrimaryProjectile()
    {
        var firing = true;

        while (firing)
        {
            var ticks = DateTime.Now.Ticks;

            if (ticks >= targetTicks)
            {
                FireProjectiles(ProjectileMode.Single, ProjectileController.Type.MEDIUM_BULLET);
                targetTicks = ticks + (projectilesDelayMs * TimeSpan.TicksPerMillisecond);
            }

            firing = !Input.GetButtonUp("Fire1");
            yield return null;
        }
    }

    private IEnumerator Co_FireSecondaryProjectile()
    {
        bool firing = true;

        while (firing)
        {
            var ticks = DateTime.Now.Ticks;

            if (ticks >= targetTicks)
            {
                FireProjectiles(ProjectileMode.Dual, ProjectileController.Type.HEAVY_PROTON);
                targetTicks = ticks + (projectilesDelayMs * TimeSpan.TicksPerMillisecond);
            }

            firing = !Input.GetButtonUp("Fire2");
            yield return null;
        }
    }

    private Vector3 CalculateKeyInputPosition()
    {
        var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        var regulatedInput = input * Time.deltaTime * speed;

        animator.SetFloat("speed", input.x);

        SetExhaustThrust(input.y);

        reverseLeftTrust.SetActive(input.y < 0.0f);
        reverseRightTrust.SetActive(input.y < 0.0f);

        var positionX = transform.position.x + regulatedInput.x;
        var unitPositionX = Mathf.Clamp(positionX, boundary.XMin + 1.0f, boundary.XMax - 1.0f);

        var positionY = transform.position.y + regulatedInput.y;
        var unitPositionY = Mathf.Clamp(positionY, boundary.YMin + 1.25f, boundary.YMax - 1.0f);

        return new Vector3(unitPositionX, unitPositionY, transform.position.z);
    }

    private void ConfigureMovementBoundaries()
    {
        var mainCamera = Camera.main;

        boundary = new Boundary
        {
            XMin = mainCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f)).x,
            XMax = mainCamera.ViewportToWorldPoint(new Vector3(1.0f, 0.0f, 0.0f)).x,
            YMin = mainCamera.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f)).y,
            YMax = mainCamera.ViewportToWorldPoint(new Vector3(0.0f, 1.0f, 0.0f)).y
        };
    }

    private void ResolveComponents()
    {
        animator = ship.GetComponent<Animator>();
        healthAttributes = GetComponent<HealthAttributes>();
        exhaustComponents = new List<ExhaustComponent>();

        foreach (var exhaust in exhausts)
        {
            exhaustComponents.Add(new ExhaustComponent
            {
                ExhaustController = exhaust.GetComponent<DynamicExhaustController>()
            });
        }
    }

    private void FireProjectiles(ProjectileMode mode, ProjectileController.Type type)
    {
        switch (mode)
        {
            case ProjectileMode.Single:
                SpawnPrimaryProjectile(type);
                break;

            case ProjectileMode.Dual:
                SpawnWingProjectiles(type);
                break;

            case ProjectileMode.All:
                SpawnPrimaryProjectile(type);
                SpawnWingProjectiles(type);
                break;
        }
    }

    private IList<GameObject> SpawnPrimaryProjectile(ProjectileController.Type type) => new List<GameObject>{SpawnProjectileAndActuate(type, new Vector2(transform.position.x, transform.position.y + 0.8f))};

    private IList<GameObject> SpawnWingProjectiles(ProjectileController.Type type)
    {
        return new List<GameObject>{
            SpawnProjectileAndActuate(type, new Vector2(transform.position.x - 0.5f, transform.position.y)),
            SpawnProjectileAndActuate(type, new Vector2(transform.position.x + 0.5f, transform.position.y))
        };
    }

    private GameObject SpawnProjectileAndActuate(ProjectileController.Type type, Vector2 position)
    {
        var projectile = SpawnProjectile(type, position);
        var velocityProjectileController = projectile.GetComponent<VelocityProjectileController>();

        velocityProjectileController.Actuate(new VelocityProjectileController.Configuration
        {
            Layer = layer,
            Direction = new Vector2(0.0f, 1.0f)
        });

        return projectile;
    }

    private GameObject SpawnProjectile(ProjectileController.Type type, Vector2 position)
    {
        GameObject projectilePrefab = null;

        switch (type)
        {
            case ProjectileController.Type.LIGHT_BULLET:
                projectilePrefab = prefabs.lightBullet;
                break;

            case ProjectileController.Type.LIGHT_PROTON:
                projectilePrefab = prefabs.lightProton;
                break;

            case ProjectileController.Type.MEDIUM_BULLET:
                projectilePrefab = prefabs.mediumBullet;
                break;

            case ProjectileController.Type.MEDIUM_PROTON:
                projectilePrefab = prefabs.mediumProton;
                break;

            case ProjectileController.Type.HEAVY_BULLET:
                projectilePrefab = prefabs.heavyBullet;
                break;

            case ProjectileController.Type.HEAVY_PROTON:
                projectilePrefab = prefabs.heavyProton;
                break;
        }

        var projectile = Instantiate(projectilePrefab, new Vector3(position.x, position.y, projectilePrefab.transform.position.z), Quaternion.identity);
        projectile.name = $"{projectilePrefab.name}-{Signature}";
        projectile.layer = (int) layer;

        return projectile;
    }

    public void Reset() => transform.position = defaultPlayerPosition;

    private void OnShipEngagedComplete() => delegates?.OnShipEngagedDelegate?.Invoke();

    private void OnShipDisengagedComplete() => delegates?.OnShipDisengagedDelegate?.Invoke();

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            var trigger = collider.gameObject;

            if (trigger.tag.Equals("Projectile"))
            {
                if (trigger.name.Contains(Signature))
                {
                    // Ignore as it's signature matches that of the game object instance
                    return;
                }

                Destroy(trigger);
            }

            var damageAttributes = trigger.GetComponent<DamageAttributes>();

            if (damageAttributes != null)
            {
                var damageMetric = damageAttributes.GetDamageMetric();
                healthAttributes.SubstractHealth(damageMetric);

                if (healthAttributes.GetHealthMetric() > 0.0f)
                {
                    StartCoroutine(Co_ManifestDamage());
                }
                else
                {
                    Debug.Log($"You lost a life!!!");
                    // TODO Life Lost
                }
            }
        }
    }

    private IEnumerator Co_ManifestDamage()
    {
        var spriteRenderers = new List<SpriteRenderer>();

        foreach (Transform childTransform in transform)
        {
            var spriteRenderer = childTransform.GetComponent<SpriteRenderer>();

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