using System;
using System.Collections;

using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
public class ShipDefenceController : BaseMonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] float turnSpeed = 10.0f;
    [SerializeField] long projectilesDelayMs = 250;

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

    [SerializeField] Prefabs prefabs;

    private HealthAttributes healthAttributes;
    private GUIAttributes guiAttributes;
    private long targetTicks;
    private RenderLayer layer;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();

        layer = RenderLayer.GAMEPLAY;
    }

    // Update is called once per frame
    void Update()
    {
        float movement = Input.GetAxis("Horizontal") * Time.deltaTime * turnSpeed;

        if (movement != 0.0f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - (movement * Time.deltaTime * turnSpeed * 90.0f));

            if (camera != null)
            {
                //camera.transform.rotation = transform.rotation;
                camera.transform.rotation = Quaternion.Euler(camera.transform.rotation.eulerAngles.x, camera.transform.rotation.eulerAngles.y, camera.transform.rotation.eulerAngles.z - (movement * Time.deltaTime * turnSpeed * 45.0f));
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Co_Fire());
        }
    }

    private void ResolveComponents()
    {
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
        guiAttributes = GetComponent<GUIAttributes>() as GUIAttributes;
    }

    private IEnumerator Co_Fire()
    {
        bool firing = true;

        while (firing)
        {
            long ticks = DateTime.Now.Ticks;

            if (ticks >= targetTicks)
            {
                FireProjectile(ProjectileController.Type.LIGHT_BULLET);
                targetTicks = ticks + (projectilesDelayMs * TimeSpan.TicksPerMillisecond);
            }

            firing = !(Input.GetButtonUp("Fire1"));
            yield return null;
        }
    }

    private GameObject FireProjectile(ProjectileController.Type type)
    {
        GameObject projectile = SpawnProjectile(type, new Vector2(transform.position.x, transform.position.y));
        projectile.transform.rotation = transform.rotation;
        //projectile.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        //if (guiAttributes != null)
        //{
        //    guiAttributes.SetAttribute("rotation", $"Rotation: {transform.rotation.eulerAngles}");
        //    guiAttributes.SetAttribute("projectile_rotation", $"Projectile Rotation: {projectile.transform.rotation.eulerAngles}");
        //}

        var velocityProjectileController = projectile.GetComponent<VelocityProjectileController>() as VelocityProjectileController;

        velocityProjectileController.Actuate(new VelocityProjectileController.Configuration
        {
            Layer = layer,
            Direction = VectorFunctions.ToVector2(projectile.transform.rotation * Quaternion.Euler(0.0f, 0.0f, 90.0f) * Vector3.right)
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

            case ProjectileController.Type.LIGHT_MISSILE:
                projectilePrefab = prefabs.lightProton;
                break;

            case ProjectileController.Type.MEDIUM_BULLET:
                projectilePrefab = prefabs.mediumBullet;
                break;

            case ProjectileController.Type.MEDIUM_MISSILE:
                projectilePrefab = prefabs.mediumProton;
                break;

            case ProjectileController.Type.HEAVY_BULLET:
                projectilePrefab = prefabs.heavyBullet;
                break;

            case ProjectileController.Type.HEAVY_MISSILE:
                projectilePrefab = prefabs.heavyProton;
                break;
        }

        var projectile = Instantiate(projectilePrefab, new Vector3(position.x, position.y, projectilePrefab.transform.position.z), Quaternion.identity) as GameObject;
        projectile.name = $"{projectilePrefab.name}-{Signature}";
        projectile.layer = (int) layer;

        return projectile;
    }
}