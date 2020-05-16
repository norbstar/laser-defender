using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
public class PlayerController : /*GUI*/MonoBehaviour
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
    [SerializeField] GameObject exhaust;
    [SerializeField] GameObject maxExhaust;
    [SerializeField] GameObject reverseLeftExhaust;
    [SerializeField] GameObject reverseRightExhaust;
    [SerializeField] Prefabs prefabs;
    [SerializeField] float speed = 10.0f, transitionSpeed = 2.5f;
    [SerializeField] long projectilesDelayMs = 250;
    
    public class Boundary
    {
        public float XMin { get; set; }
        public float XMax { get; set; }
        public float YMin { get; set; }
        public float YMax { get; set; }

        public override string ToString()
        {
            return $"({XMin}, {YMin}, {XMax}, {YMax})";
        }
    }

    public enum ProjectileMode
    {
        SINGLE,
        DUAL,
        ALL
    }

    private Rigidbody2D rigidBody;
    private Animator animator;
    private Delegates delegates;
    private Boundary boundary;
    private long targetTicks;
    private bool controlsActive = false;
    private Vector3 defaultPlayerPosition;
    private OnShipDisengaged onShipDisengagedDelegate;
    private AudioSource exhaustAudioSource;
    private float inputSpeedX, inputSpeedY;

    void Awake()
    {
        ResolveComponents();
        //ConfigGUIStyle();

        //guiStyle.fontSize = 8;
        //guiStyle.font = (Font) Resources.Load("Fonts/Block Stock");
        //guiStyle.normal.textColor = Color.white;

        ConfigureMovementBoundaries();
    }

    // Start is called before the first frame update
    void Start()
    {
        defaultPlayerPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ControlsEnabled())
        {
            return;
        }

        transform.position = CalculateKeyInputPosition();

        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(FireProjectilesCoroutine());
        }
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    public float GetThrust()
    {
        //return (rigidBody.velocity.y == 0) ? 1.0f : rigidBody.velocity.y;

        if (inputSpeedY != 0.0f)
        {
            return inputSpeedY;
        }

        return 1.0f;
    }

    public GameObject GetExhaust()
    {
        return exhaust;
    }

    public GameObject GetMaxExhaust()
    {
        return maxExhaust;
    }

    public GameObject GetReverseLeftExhaust()
    {
        return reverseLeftExhaust;
    }

    public GameObject GetReverseRightExhaust()
    {
        return reverseRightExhaust;
    }

    public void EnableControls()
    {
        controlsActive = true;
    }

    public void DisableControls()
    {
        controlsActive = false;
    }

    public bool ControlsEnabled()
    {
        return controlsActive;
    }

    public void EngageShip()
    {
        StartCoroutine(EngageShipCoroutine());
    }

    //public IEnumerator EngageShip()
    //{
    //    yield return StartCoroutine(EngageShipCoroutine());
    //}

    private IEnumerator EngageShipCoroutine()
    {
        Vector3 originPosition = transform.position;
        Vector3 targetPosition = new Vector3(originPosition.x, 1.5f, originPosition.z);
        float magnitude = (targetPosition - originPosition).magnitude * 0.5f;
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) * (transitionSpeed / magnitude);
            transform.position = Vector3.Lerp(originPosition, targetPosition, (float) fractionComplete);

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnShipEngagedComplete();
            }

            yield return null;
        }
    }

    public IEnumerator DisengageShip()
    {
        yield return StartCoroutine(DisengageShipCoroutine());
    }

    IEnumerator DisengageShipCoroutine()
    {
        Vector3 originPosition = transform.position;
        Vector3 targetPosition = new Vector3(originPosition.x, 17, originPosition.z);
        float magnitude = (targetPosition - originPosition).magnitude * 0.5f;
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) * (transitionSpeed / magnitude);
            transform.position = Vector3.Lerp(originPosition, targetPosition, (float) fractionComplete);

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnShipDisengagedComplete();
            }

            yield return null;
        }
    }

    IEnumerator FireProjectilesCoroutine()
    {
        bool firing = true;

        while (firing)
        {
            long ticks = DateTime.Now.Ticks;

            if (ticks >= targetTicks)
            {
                ProjectileMode mode = (ProjectileMode) Enum.GetValues(typeof(ProjectileMode)).GetValue(UnityEngine.Random.Range(0, Enum.GetValues(typeof(ProjectileMode)).Length));
                ProjectileController.Type type = (ProjectileController.Type) Enum.GetValues(typeof(ProjectileController.Type)).GetValue(UnityEngine.Random.Range(0, Enum.GetValues(typeof(ProjectileController.Type)).Length));
                FireProjectiles(mode, type);
                targetTicks = ticks + (projectilesDelayMs * TimeSpan.TicksPerMillisecond);
            }

            firing = !(Input.GetButtonUp("Fire1"));
            yield return null;
        }
    }

    private Vector3 CalculateKeyInputPosition()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * speed;

        inputSpeedX = input.x * 6.5f;
        inputSpeedY = input.y * 6.5f;

        if (input.x != 0.0f)
        {
            animator?.SetFloat("speed", inputSpeedX);
        }

        if (input.y > 0.0f)
        {
            exhaust.SetActive(false);
            maxExhaust.SetActive(true);
            reverseLeftExhaust.SetActive(false);
            reverseRightExhaust.SetActive(false);
        }
        else if (input.y < 0.0f)
        {
            exhaust.SetActive(false);
            maxExhaust.SetActive(false);
            reverseLeftExhaust.SetActive(true);
            reverseRightExhaust.SetActive(true);
        }
        else
        {
            exhaust.SetActive(true);
            maxExhaust.SetActive(false);
            reverseLeftExhaust.SetActive(false);
            reverseRightExhaust.SetActive(false);
        }

        var positionX = transform.position.x + input.x;
        //float unitPositionX = Mathf.Clamp(positionX, 1.0f, InGameManager.ScreenWidthInUnits - 1.0f);
        float unitPositionX = Mathf.Clamp(positionX, boundary.XMin + 1.0f, boundary.XMax - 1.0f);

        var positionY = transform.position.y + input.y;
        //float unitPositionY = Mathf.Clamp(positionY, 1.25f, InGameManager.ScreenHeightInUnits - 1.0f);
        float unitPositionY = Mathf.Clamp(positionY, boundary.YMin + 1.25f, boundary.YMax - 1.0f);

        return new Vector3(unitPositionX, unitPositionY, transform.position.z);
    }

    private Vector3 CalculateMouseInputPosition()
    {
        Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 mousePosition = Input.mousePosition;

        float relativePositionX = mousePosition.x / Screen.width;
        //float unitPositionX = relativePositionX * InGameManager.ScreenWidthInUnits;
        float unitPositionX = Mathf.Clamp(relativePositionX, boundary.XMin + 1.0f, boundary.XMax - 1.0f);
        unitPositionX = Mathf.Clamp(unitPositionX, 1.0f, InGameManagerOld.ScreenWidthInUnits - 1.0f);

        float relativePositionY = mousePosition.y / Screen.height;
        //float unitPositionY = relativePositionY * InGameManager.ScreenHeightInUnits;
        float unitPositionY = Mathf.Clamp(relativePositionY, boundary.YMin + 1.25f, boundary.YMax - 1.0f);
        unitPositionY = Mathf.Clamp(unitPositionY, 1.25f, InGameManagerOld.ScreenHeightInUnits - 1.0f);

        return new Vector3(unitPositionX, unitPositionY, transform.position.z);
    }

    private void ConfigureMovementBoundaries()
    {
        Camera mainCamera = Camera.main;

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
        rigidBody = GetComponent<Rigidbody2D>();
        animator = ship.GetComponent<Animator>();
        exhaustAudioSource = exhaust.GetComponent<AudioSource>();
    }

    private void FireProjectiles(ProjectileMode mode, ProjectileController.Type type)
    {
        switch (mode)
        {
            case ProjectileMode.SINGLE:
                SpawnPrimaryProjectile(type);
                break;

            case ProjectileMode.DUAL:
                SpawnWingProjectiles(type);
                break;

            case ProjectileMode.ALL:
                SpawnPrimaryProjectile(type);
                SpawnWingProjectiles(type);
                break;
        }
    }

    private IList<GameObject> SpawnPrimaryProjectile(ProjectileController.Type type)
    {
        IList<GameObject> shots = new List<GameObject>();
        shots.Add(SpawnProjectile(type, new Vector2(transform.position.x, transform.position.y + 0.8f)));

        return shots;
    }

    private IList<GameObject> SpawnWingProjectiles(ProjectileController.Type type)
    {
        IList<GameObject> shots = new List<GameObject>();
        shots.Add(SpawnProjectile(type, new Vector2(transform.position.x - 0.5f, transform.position.y)));
        shots.Add(SpawnProjectile(type, new Vector2(transform.position.x + 0.5f, transform.position.y)));

        return shots;
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

        return Instantiate(projectilePrefab, new Vector3(position.x, position.y, projectilePrefab.transform.position.z), Quaternion.identity) as GameObject;
    }

    public void Reset()
    {
        transform.position = defaultPlayerPosition;
    }

    private void OnShipEngagedComplete()
    {
        delegates?.OnShipEngagedDelegate?.Invoke();
    }

    private void OnShipDisengagedComplete()
    {
        delegates?.OnShipDisengagedDelegate?.Invoke();
    }

    //public override void SetGUIAttributes(GUIStyle guiStyle)
    //{
    //    GUI.Label(new Rect(20, 25, 200, 40), $"InputSpeed X: {inputSpeedX}", guiStyle);
    //    GUI.Label(new Rect(20, 50, 200, 40), $"InputSpeed Y: {inputSpeedY}", guiStyle);
    //}
}