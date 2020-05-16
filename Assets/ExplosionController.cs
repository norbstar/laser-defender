using UnityEngine;

public class ExplosionController : MonoBehaviour, IActuate
{
    [Header("Particle System")]
    [SerializeField] GameObject particalSystemPrefab;
    [SerializeField] bool enableParticalEffects = false;
    
    [Header("Propagation Wave")]
    [SerializeField] GameObject propagationWavePrefab;
    [SerializeField] float propagationSpeed;

    public class Configuration : GameplayConfiguration
    {
        public float Range { get; set; }
        public float Speed { get; set; }
        public float DamageMetric { get; set; }
    }

    private Animator animator;
    private float damageMetric;
    private float range;
    private RenderLayer layer;

    void Awake()
    {
        ResolveComponents();
    }
    private void ResolveComponents()
    {
        animator = GetComponent<Animator>() as Animator;
    }

    public void Actuate(IConfiguration configuration)
    {
        animator.SetBool("actuate", true);

        if (configuration != null)
        {
            if (typeof(GameplayConfiguration).IsInstanceOfType(configuration))
            {
                layer = ((GameplayConfiguration) configuration).Layer;
            }

            if (typeof(Configuration).IsInstanceOfType(configuration))
            {
                range = ((Configuration) configuration).Range;
                propagationSpeed = ((Configuration) configuration).Speed;
                damageMetric = ((Configuration) configuration).DamageMetric;
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

        var propagationWave = Instantiate(propagationWavePrefab, gameObject.transform.position, Quaternion.identity) as GameObject;
        var propagationWaveController = propagationWave.GetComponent<PropagationWaveController>() as PropagationWaveController;

        propagationWaveController.Actuate(new PropagationWaveController.Configuration
        {
            Layer = layer,
            Range = range,
            Speed = propagationSpeed,
            DamageMetric = damageMetric
        });
    }
}