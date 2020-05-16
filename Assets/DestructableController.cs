using UnityEngine;

[RequireComponent(typeof(HealthAttributes))]
public abstract class DestructableController : MonoBehaviour, IActuate, IState, INotify
{
    public abstract void Actuate(IConfiguration configuration);

    public delegate void OnDestructableDamaged(GameObject gameObject, HealthAttributes healthAttributes);
    public delegate void OnDestructableDestroyed(GameObject gameObject);

    public class Delegates
    {
        public OnDestructableDamaged OnDestructableDamagedDelegate { get; set; }
        public OnDestructableDestroyed OnDestructableDestroyedDelegate { get; set; }
    }

    [Header("Common")]
    [SerializeField] Mode mode = Mode.ACTIVE;

    [Header("Health")]
    [SerializeField] protected HealthBarSliderUIManager healthBarSliderUIManager;

    protected Delegates delegates;
    protected HealthAttributes healthAttributes;
    protected RenderLayer layer;
    protected Mode activeMode;

    public virtual void Awake()
    {
        ResolveDestructableComponents();

        activeMode = Mode.INACTIVE;
    }

    private void ResolveDestructableComponents()
    {
        healthAttributes = GetComponent<HealthAttributes>() as HealthAttributes;
    }

    public virtual void SetActive(bool active)
    {
        activeMode = (active) ? Mode.ACTIVE : Mode.INACTIVE;
    }

    public bool IsActive()
    {
        return (activeMode == Mode.ACTIVE);
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    public void OnLayerChange(int layer)
    {
        if ((gameObject.activeSelf) && (mode == Mode.ACTIVE))
        {
            SetActive((mode == Mode.ACTIVE) && (gameObject.layer == layer));
        }
    }
}