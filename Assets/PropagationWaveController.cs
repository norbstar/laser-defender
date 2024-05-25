using System.Collections;

using UnityEngine;

public class PropagationWaveController : MonoBehaviour, IActuate
{
    public delegate void OnPropagationWaveComplete();

    private OnPropagationWaveComplete onPropagationWaveComplete;

    public class Configuration : GameplayConfiguration
    {
        public float Range { get; set; }
        public float Speed { get; set; }
        public float DamageMetric { get; set; }
    }

    [SerializeField] float propagationSpeed;

    private new SpriteRenderer renderer;
    private float damageMetric;
    private float range;
    private RenderLayer layer;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        renderer = GetComponent<SpriteRenderer>() as SpriteRenderer;
    }

    public void RegisterDelegate(OnPropagationWaveComplete onPropagationWaveComplete)
    {
        this.onPropagationWaveComplete = onPropagationWaveComplete;
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
                this.range = ((Configuration) configuration).Range;
                this.propagationSpeed = ((Configuration) configuration).Speed;
                this.damageMetric = ((Configuration) configuration).DamageMetric;
            }
        }

        StartCoroutine(Co_Acuate());
    }

    private IEnumerator Co_Acuate()
    {
        gameObject.layer = (int) layer;

        int sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        Vector3 originScale = transform.localScale;
        Color originColor = renderer.color;

        Vector3 targetScale = transform.localScale * (range * 2.0f);
        Color targetColor = new Color(originColor.r, originColor.g, originColor.b, 0.0f);
        
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * (propagationSpeed * 5.0f);

            if (fractionComplete >= 0.0f)
            {
                transform.localScale = Vector3.Lerp(originScale, targetScale, fractionComplete);
                renderer.material.SetColor("_Color", Color.Lerp(originColor, targetColor, fractionComplete));

                complete = fractionComplete >= 1f;
            }

            if (complete)
            {
                Destroy(gameObject);
                onPropagationWaveComplete?.Invoke();
            }

            yield return null;
        }
    }

    public float GetDamageMetric()
    {
        float distance = transform.localScale.x * 0.5f;
        float fraction = NormalizeValue(distance, range);

        return damageMetric * fraction;
    }

    private float NormalizeValue(float value, float maxValue)
    {
        return (value == maxValue) ? 0.0f : (maxValue - value) / value;
    }
}