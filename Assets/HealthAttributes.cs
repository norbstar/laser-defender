using UnityEngine;

public class HealthAttributes : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] float healthMetric;
    [SerializeField] bool invulnerable;

    public float HealthMetric => healthMetric;

    public void SubstractHealth(float healthMetric)
    {
        if (!invulnerable)
        {
            this.healthMetric -= healthMetric;
        }
    }
}