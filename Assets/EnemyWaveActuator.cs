using System.Collections;

using UnityEngine;

public class EnemyWaveActuator : MonoBehaviour, IActuate
{
    [SerializeField] WaveConfig waveConfig;

    private GameObject enemyPrefab;
    private float timeBetweenSpawns;
    private int waveSize, enemyCount;
    private GeneralResources generalResources;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents() {
        generalResources = FindObjectOfType<GeneralResources>();
    }

    private void ResolveDependencies()
    {
        enemyPrefab = waveConfig.GetEnemyPrefab();
        timeBetweenSpawns = waveConfig.GetTimeBetweenSpawns();
        waveSize = waveConfig.GetWaveSize();
    }

    public void Actuate(IConfiguration configuration = null)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(Co_Actuate());
        }
    }

    private IEnumerator Co_Actuate()
    {
        ResolveDependencies();

        for (int itr = 0; itr < waveSize; ++itr)
        {
            GameObject prefab = Instantiate(enemyPrefab, new Vector3(-1.0f, -1.0f, 1.0f), Quaternion.identity) as GameObject;
            prefab.transform.parent = transform;

            var enemyController = prefab.GetComponent<EnemyController>() as EnemyController;
            enemyController.Configure(waveConfig, 0);
            enemyController.RegisterDelegates(new EnemyController.Delegates
            {
                OnEnemyDamagedDelegate = OnEnemyDamaged,
                OnEnemyDestroyedDelegate = OnEnemyDestroyed
            });

            enemyController.Actuate(new EnemyController.Configuration { });
            ++enemyCount;

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    public void OnEnemyDamaged(GameObject gameObject, int waveSequence, HealthAttributes healthAttributes)
    {
        // TODO
    }

    public void OnEnemyDestroyed(GameObject gameObject, int waveSequence, bool selfAdministered)
    {
        if (!selfAdministered)
        {
            Destroy(Instantiate(generalResources.ExplosionPrefab, gameObject.transform.position, Quaternion.identity), 0.15f);
        }

        --enemyCount;
        Destroy(gameObject);

        if (enemyCount == 0)
        {
            Destroy(this.gameObject);
        }
    }
}