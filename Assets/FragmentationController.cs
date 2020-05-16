using UnityEngine;

public class FragmentationController : MonoBehaviour, IActuate
{
    [Header("Particle System")]
    [SerializeField] GameObject particalSystemPrefab;
    [SerializeField] bool enableParticalEffects = false;

    public class Configuration : GameplayConfiguration
    {
        public Transform RefTransform { get; set; }
        public float IntersectionAngle { get; set; }
        public GameObject Prefab { get; set; }
    }

    private Transform refTransform;
    private float intersectionAngle;
    private GameObject prefab;
    private RenderLayer layer;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents() { }

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
                intersectionAngle = ((Configuration) configuration).IntersectionAngle;
                prefab = ((Configuration) configuration).Prefab;
            }
        }

        if (enableParticalEffects)
        {
            var particalSystem = Instantiate(particalSystemPrefab, gameObject.transform.position, particalSystemPrefab.transform.rotation) as GameObject;
            particalSystem.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.x) * 0.5f;

            var particleSystemRenderer = particalSystem.GetComponent<ParticleSystemRenderer>() as ParticleSystemRenderer;
            int sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
            particleSystemRenderer.sortingLayerID = sortingOrderId;

            Destroy(particalSystem, 1.0f);
        }

        // TODO optional sub asteroid spawn with velocity

        Vector2 vector = (Vector2) (Quaternion.Euler(0.0f, 0.0f, intersectionAngle) * Vector2.right);
        Debug.Log($"Vector: {vector}");

        //var asteroid = Instantiate(prefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        //asteroid.transform.parent = refTransform;
        //float scale = refTransform.localScale.x * 0.5f;
        //asteroid.transform.localScale = new Vector3(scale, scale, 1.0f);

        //var asteroidController = asteroid.GetComponent<VelocityAsteroidController>() as VelocityAsteroidController;

        //if (asteroidController != null)
        //{
        //    asteroidController.RegisterDelegates(new VelocityAsteroidController.Delegates
        //    {
        //        OnAsteroidDamagedDelegate = OnAsteroidDamaged,
        //        OnAsteroidDestroyedDelegate = OnAsteroidDestroyed,
        //        OnAsteroidJourneyCompleteDelegate = OnAsteroidJourneyComplete
        //    });

        //    asteroidController.Actuate(new VelocityAsteroidController.Configuration
        //    {
        //        Layer = Layer.SUB_SURFACE,
        //        Mode = VelocityAsteroidController.Mode.Active,
        //        StartTransformTime = Time.time,
        //        Vector = vector,
        //        Speed = speed,
        //        Rotation = rotation
        //    });
        //}
    }
}