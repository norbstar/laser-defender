using UnityEngine;

public class EnemyWaveActuationDemo : MonoBehaviour
{
    private EnemyWaveActuator enemyWaveActuator;

    void Awake()
    {
        ResolveComponents();
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyWaveActuator.Actuate();
    }

    private void ResolveComponents()
    {
        enemyWaveActuator = GetComponent<EnemyWaveActuator>() as EnemyWaveActuator;
    }
}