using UnityEngine;

public class DamageAttributes : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float damageMetric;

    public virtual float DamageMetric => damageMetric;
    
    public void SetDamageMetric(float damageMetric) => this.damageMetric = damageMetric;
}