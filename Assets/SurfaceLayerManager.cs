using UnityEngine;

public class SurfaceLayerManager : MonoBehaviour, IActuate
{
    [Header("Surface Layer")]
    [SerializeField] [ShowOnly] protected float scaleFactor = 1.0f;

    public class Configuration : IConfiguration
    {
        public float ZDepth { get; set; }
    }

    private float zDepth = 0.0f;

    public void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration = null)
    {
        if (configuration != null)
        {
            if (typeof(Configuration).IsInstanceOfType(configuration))
            {
                zDepth = ((Configuration) configuration).ZDepth;
            }
        }

        scaleFactor = 1.0f / (transform.position.z + zDepth);
    }
}