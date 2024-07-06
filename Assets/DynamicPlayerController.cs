using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(HealthAttributes))]
public class DynamicPlayerController : BaseMonoBehaviour
{
    [Header("Config")]
    [SerializeField] InputAction moveX;
    [SerializeField] InputAction moveY;
    [SerializeField] InputAction primaryFire;
    [SerializeField] InputAction secondaryFire;
    [SerializeField] InputAction switchMode;
    // [SerializeField] InputActionMap inputActions;
    [SerializeField] float targetRadius = 5f;

    // [Header("Analytics")]
    // [SerializeField] List<Collider2D> colliders;

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
        public GameObject lightMissile;
        public GameObject seekingLightMissile;
        public GameObject mediumMissile;
        public GameObject seekingMediumMissile;
        public GameObject heavyMissile;
        public GameObject seekingHeavyMissile;
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

    public enum ProjectileCategory
    {
        Guns,
        Missiles,
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
    private ProjectileController.Mode projectileMode;
    private DualInputActions inputActions;
    private ActuatorManager actuatorManager;
    private int targetLayerMask;
    private GameObject target;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();
        ConfigureMovementBoundaries();

        layer = (RenderLayer) gameObject.layer;
        inputActions = new DualInputActions();
        actuatorManager = FindObjectOfType<ActuatorManager>(); // ActuatorManager.Instance;
        targetLayerMask = LayerMask.GetMask("Gameplay Layer");
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultPlayerPosition = transform.position;
        EngageShip();
    }

    void OnEnable()
    {
        // moveX.Enable();
        // moveX.performed += Callback_OnMoveX;

        // moveY.Enable();
        // moveY.performed += Callback_OnMoveY;

        // primaryFire.Enable();
        // primaryFire.performed += Callback_OnPrimaryFire;

        // secondaryFire.Enable();
        // secondaryFire.performed += Callback_OnSecondaryFire;

        // switchMode.Enable();
        // switchMode.performed += Callback_OnSwitchMode;

        inputActions.Enable();
        inputActions.Player.Fire.performed += Callback_OnPrimaryFire;
        inputActions.Player.AltFire.performed += Callback_OnSecondaryFire;
    }

    void OnDisable()
    {
        // moveX.Disable();
        // moveY.Disable();
        // primaryFire.Disable();
        // secondaryFire.Disable();
        // switchMode.Disable();
        inputActions.Disable();
    }

    // private void Callback_OnMoveX(InputAction.CallbackContext context)
    // {
    //     var value = context.ReadValue<float>();
    //     var moveX = value * Time.deltaTime * speed;

    //     animator.SetFloat("speed", value);

    //     var position = transform.position;
    //     var positionX = position.x + moveX;
    //     var unitPositionX = Mathf.Clamp(positionX, boundary.XMin + 1.0f, boundary.XMax - 1.0f);

    //     position.x = unitPositionX;
    //     transform.position = position;
    // }

    // private void Callback_OnMoveY(InputAction.CallbackContext context)
    // {
    //     var value = context.ReadValue<float>();
    //     var moveY = value * Time.deltaTime * speed;

    //     SetExhaustThrust(value);

    //     reverseLeftTrust.SetActive(value < 0.0f);
    //     reverseRightTrust.SetActive(value < 0.0f);

    //     var position = transform.position;
    //     var positionY = position.y + moveY;
    //     var unitPositionY = Mathf.Clamp(positionY, boundary.YMin + 1.25f, boundary.YMax - 1.0f);

    //     position.y = unitPositionY;
    //     transform.position = position;
    // }

    private void Callback_OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (!ControlsEnabled()) return;
        StartCoroutine(Co_FireBullets());
    }

    private void Callback_OnSecondaryFire(InputAction.CallbackContext context)
    {
        if (!ControlsEnabled()) return;
        StartCoroutine(Co_FireMissiles());
    }

    // private void Callback_OnSwitchMode(InputAction.CallbackContext context)
    // {
    //     if (!ControlsEnabled()) return;
    //     SwitchMode();
    // }

    private Vector3 CalculatePosition()
    {
        // var input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // var input = new Vector2(moveX.ReadValue<float>(), moveY.ReadValue<float>());
        var input = inputActions.Player.Move.ReadValue<Vector2>();
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

    // private void FilterTargetCandidates(Collider2D[] colliders)
    // {
        // this.colliders.Clear();

        // foreach (var collider in colliders)
        // {
        //     var dotProduct = Vector2.Dot(Vector2.up, (collider.transform.position - transform.position).normalized);
        //     // Debug.Log($"Target: {collider.name} Dot Product: {dotProduct}");

        //     if (dotProduct >= 0.7f && dotProduct <= 1f && collider.tag.Equals("Mine"))
        //     {
        //         // Debug.Log($"Target: {collider.name} Dot Product: {dotProduct}");
        //         this.colliders.Add(collider);
        //     }
        // }

    //     this.colliders = colliders.ToList();
    // }

    // Update is called once per frame
    void Update()
    {
        if (!ControlsEnabled()) return;

        transform.position = CalculatePosition();

        // if (Input.GetButtonDown("Fire1"))
        // {
        //     StartCoroutine(Co_FireBullets());
        // }

        // if (Input.GetButtonDown("Fire2"))
        // {
        //     StartCoroutine(Co_FireMissiles());
        // }

        // if (Input.GetButtonDown("Fire3"))
        // {
        //     SwitchMode();
        // }

        // float dotProduct;
        
        // dotProduct = Vector2.Dot(Vector2.up, Vector2.up);
        // Debug.Log($"Update Up/Up: {dotProduct}");

        // dotProduct = Vector2.Dot(Vector2.up, Vector2.down);
        // Debug.Log($"Update Up/Down: {dotProduct}");

        // dotProduct = Vector2.Dot(Vector2.up, Vector2.left);
        // Debug.Log($"Update Up/Left: {dotProduct}");

        // dotProduct = Vector2.Dot(Vector2.up, Vector2.right);
        // Debug.Log($"Update Up/Right: {dotProduct}");

        // var colliders = Physics2D.OverlapCircleAll(VectorFunctions.ToVector2(transform.position), targetRadius, targetLayerMask);
        // Debug.Log($"Update Collider Count: {colliders.Length}");
        // FilterTargetCandidates(colliders);

        if (target != null && target.TryGetComponent<IFocus>(out var priorFocus))
        {
            priorFocus.ShowCue(false);
        }

        if (inputActions.Player.Action.IsPressed())
        {
            target = ResolveTarget();

            if (target != null && target.TryGetComponent<IFocus>(out var focus))
            {
                focus.ShowCue();
            }
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

    private IEnumerator Co_FireBullets()
    {
        var firing = true;

        while (firing)
        {
            var ticks = DateTime.Now.Ticks;

            if (ticks >= targetTicks)
            {
                Fire(ProjectileCategory.Guns, ProjectileController.Type.LIGHT_BULLET);
                targetTicks = ticks + (projectilesDelayMs * TimeSpan.TicksPerMillisecond);
            }

            // firing = !Input.GetButtonUp("Fire1");
            firing = inputActions.Player.Fire.IsPressed();
            yield return null;
        }
    }

    private IEnumerator Co_FireMissiles()
    {
        bool firing = true;

        while (firing)
        {
            var ticks = DateTime.Now.Ticks;

            if (ticks >= targetTicks)
            {
                var actionPressed = inputActions.Player.Action.IsPressed();

                var type = actionPressed && target != null ? ProjectileController.Type.SEEKING_HEAVY_MISSILE : ProjectileController.Type.HEAVY_MISSILE;
                Fire(ProjectileCategory.Missiles, type);
                targetTicks = ticks + (projectilesDelayMs * TimeSpan.TicksPerMillisecond);
            }

            // firing = !Input.GetButtonUp("Fire2");
            firing = inputActions.Player.AltFire.IsPressed();
            yield return null;
        }
    }

    // private void SwitchMode() => projectileMode = projectileMode == ProjectileController.Mode.NORMAL ? ProjectileController.Mode.SEEKING : ProjectileController.Mode.NORMAL;

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

    private void Fire(ProjectileCategory category, ProjectileController.Type type)
    {
        switch (category)
        {
            case ProjectileCategory.Guns:
                FireGuns(type);
                break;

            case ProjectileCategory.Missiles:
                FireMissiles(type);
                break;

            case ProjectileCategory.All:
                FireGuns(type);
                FireMissiles(type);
                break;
        }
    }

    private IList<GameObject> FireGuns(ProjectileController.Type type)
    {
        return new List<GameObject>{
            FireAction(type, new Vector2(transform.position.x - 0.275f, transform.position.y + 0.08f)),
            FireAction(type, new Vector2(transform.position.x + 0.275f, transform.position.y + 0.08f)),
        };
    }

    private IList<GameObject> FireMissiles(ProjectileController.Type type)
    {
        return new List<GameObject>{
            FireAction(type, new Vector2(transform.position.x - 0.5f, transform.position.y)),
            FireAction(type, new Vector2(transform.position.x + 0.5f, transform.position.y))
        };
    }

    private List<GameObject> ResolveValidTargets()
    {
        var validTargets = new List<GameObject>();
        var colliders = Physics2D.OverlapAreaAll(new Vector2(boundary.XMin,boundary.YMin), new Vector2(boundary.XMax,boundary.YMax), targetLayerMask);

        foreach (var target in colliders)
        {
            var dotProduct = Vector2.Dot(Vector2.up, (target.transform.position - transform.position).normalized);

            if (dotProduct >= 0.7f && dotProduct <= 1f)
            {
                validTargets.Add(target.gameObject);
            }
        }

        return validTargets;
    }

    private List<GameObject> ResolveHighestThreatTargets()
    {
        // var colliders = Physics2D.OverlapAreaAll(new Vector2(boundary.XMin,boundary.YMin), new Vector2(boundary.XMax,boundary.YMax), targetLayerMask);

        // var highestThreatColliders = new List<Collider2D>();
        // int highestThreatLevel = 0;

        // foreach (var collider in colliders)
        // {
        //     if (collider.TryGetComponent<ThreatLevelAttributes>(out var threatLevel))
        //     {
        //         var dotProduct = Vector2.Dot(Vector2.up, (collider.transform.position - transform.position).normalized);

        //         if (dotProduct >= 0.7f && dotProduct <= 1f)
        //         {
        //             if (threatLevel.ThreatLevel > highestThreatLevel)
        //             {
        //                 highestThreatColliders.Clear();
        //                 highestThreatColliders.Add(collider);
        //             }
        //             else if (threatLevel.ThreatLevel == highestThreatLevel)
        //             {
        //                 highestThreatColliders.Add(collider);
        //             }
        //         }
        //     }
        // }

        // return highestThreatColliders;

        var highestThreatTargets = new List<GameObject>();
        int highestThreatLevel = 0;

        foreach (var target in ResolveValidTargets())
        {
            if (target.TryGetComponent<ThreatLevelAttributes>(out var threatLevel))
            {
                if (threatLevel.ThreatLevel > highestThreatLevel)
                {
                    highestThreatTargets.Clear();
                    highestThreatTargets.Add(target);
                }
                else if (threatLevel.ThreatLevel == highestThreatLevel)
                {
                    highestThreatTargets.Add(target);
                }
            }
        }

        return highestThreatTargets;
    }

    private GameObject ResolveTarget()
    {
        // var actuators = actuatorManager.Actuators;
        // Debug.Log($"ResolveViableTarget Count: {actuators.Count}");

        // foreach (var actuator in actuators)
        // {
        //     var position = actuator.transform.position;
        //     Debug.Log($"ResolveViableTarget Position: {position}");
        // }

        // if (target != null && target.TryGetComponent<IFocus>(out var priorFocus))
        // {
        //     priorFocus.ShowCue(false);
        // }

        var higestThreatTargets = ResolveHighestThreatTargets();

        // float shortestDistance = float.MaxValue;

        // foreach (var target in higestThreatTargets)
        // {
        //     var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        //     if (distanceToTarget < shortestDistance)
        //     {
        //         shortestDistance = distanceToTarget;
        //         this.target = target;
        //     }
        // }

        if (higestThreatTargets.Count > 0)
        {
            target = higestThreatTargets[0];
        }

        // if (target != null && target.TryGetComponent<IFocus>(out var focus))
        // {
        //     focus.ShowCue();
        // }

        return target;
    }

    private void ActuateProjectile(GameObject projectile)
    {
        if (projectile.TryGetComponent<SeekingVelocityProjectileController>(out var seekingVelocityController))
        {
            seekingVelocityController.Signature = Signature;
            seekingVelocityController.Actuate(new SeekingVelocityProjectileController.Configuration
            {
                Layer = layer,
                Target = target
            });
        }
        else if (projectile.TryGetComponent<VelocityProjectileController>(out var velocityController))
        {
            velocityController.Signature = Signature;
            velocityController.Actuate(new VelocityProjectileController.Configuration
            {
                Layer = layer,
                // Direction = new Vector2(0.0f, 1.0f)
            });
        }
    }

    private GameObject SpawnProjectile(ProjectileController.Type type, Vector2 position)
    {
        // Debug.Log($"SpawnProjectile TYpe: {type} Position: {position}");
        GameObject projectilePrefab = null;

        switch (type)
        {
            case ProjectileController.Type.LIGHT_BULLET:
                projectilePrefab = prefabs.lightBullet;
                break;

            case ProjectileController.Type.LIGHT_MISSILE:
                projectilePrefab = prefabs.lightMissile;
                break;

            case ProjectileController.Type.SEEKING_LIGHT_MISSILE:
                projectilePrefab = prefabs.seekingLightMissile;
                break;

            case ProjectileController.Type.MEDIUM_BULLET:
                projectilePrefab = prefabs.mediumBullet;
                break;

            case ProjectileController.Type.MEDIUM_MISSILE:
                projectilePrefab = prefabs.mediumMissile;
                break;

            case ProjectileController.Type.SEEKING_MEDIUM_MISSILE:
                projectilePrefab = prefabs.seekingMediumMissile;
                break;

            case ProjectileController.Type.HEAVY_BULLET:
                projectilePrefab = prefabs.heavyBullet;
                break;

            case ProjectileController.Type.HEAVY_MISSILE:
                projectilePrefab = prefabs.heavyMissile;
                break;

            case ProjectileController.Type.SEEKING_HEAVY_MISSILE:
                projectilePrefab = prefabs.seekingHeavyMissile;
                break;
        }

        var projectile = Instantiate(projectilePrefab, new Vector3(position.x, position.y, projectilePrefab.transform.position.z), Quaternion.identity);
        projectile.name = $"{projectilePrefab.name}-{Signature}";
        projectile.layer = (int) layer;

        return projectile;
    }

    private GameObject FireAction(ProjectileController.Type type, Vector2 position)
    {
        var projectile = SpawnProjectile(type, position);
        ActuateProjectile(projectile);
        return projectile;
    }

    public void Reset() => transform.position = defaultPlayerPosition;

    private void OnShipEngagedComplete() => delegates?.OnShipEngagedDelegate?.Invoke();

    private void OnShipDisengagedComplete() => delegates?.OnShipDisengagedDelegate?.Invoke();

    private bool SharesSameSignature(GameObject obj) => obj.name.Contains(Signature);

    private void OnDestroyed()
    {
        // RELAY THE EVENT TO A HIGHER POWER AND DEPLOY A NEW SHIP
    }

    private void ApplyDamage(DamageAttributes damageAttributes)
    {
        if (damageAttributes != null)
        {
            var damageMetric = damageAttributes.DamageMetric;
            healthAttributes.SubstractHealth(damageMetric);

            if (healthAttributes.HealthMetric > 0.0f)
            {
                StartCoroutine(Co_ManifestDamage());
            }
            else
            {
                OnDestroyed();
            }
        }
    }

    private bool SharesSameLayer(GameObject obj) => obj.layer == gameObject.layer;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            var trigger = collider.gameObject;
            // bool acknowledgeTrigger = false;
            // bool destroyTrigger = false;

            // if (trigger.tag.Equals(PROJECTILE_TAG))
            // {
            //     if (HasOwnership(trigger))
            //     {
            //         destroyTrigger = true;
            //     }
            // }

            // if (destroyTrigger)
            // {
            //     Destroy(trigger);
            //     return;
            // }

            // ApplyDamage(trigger);

            // if (trigger.tag.Equals(PROJECTILE_TAG))
            // {
            //     acknowledgeTrigger = true;
            // }

            if (SharesSameLayer(collider.gameObject)) return;

            if (trigger.TryGetComponent<DamageAttributes>(out var damageAttributes))
            {
                ApplyDamage(damageAttributes);
            }

            // if (/*acknowledgeTrigger && */!SharesSameSignature(trigger))
            // {
            //     ApplyDamage(trigger);
            // }
        }
    }

    private IEnumerator Co_ManifestDamage()
    {
        var spriteRenderers = new List<SpriteRenderer>();

        foreach (Transform childTransform in transform)
        {
            var spriteRenderer = childTransform.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null && spriteRenderer.gameObject.activeSelf)
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