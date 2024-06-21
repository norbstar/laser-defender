using UnityEngine;

[RequireComponent(typeof(PropagationWaveController))]
public class PropagationWaveDamageAttributes : DamageAttributes
{
    private PropagationWaveController propagationWaveController;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        propagationWaveController = GetComponent<PropagationWaveController>() as PropagationWaveController;
    }

    public override float DamageMetric
    {
        get
        {
            float damageMetric = propagationWaveController.GetDamageMetric();
            SetDamageMetric(damageMetric);

            return damageMetric;
        }
    }
}