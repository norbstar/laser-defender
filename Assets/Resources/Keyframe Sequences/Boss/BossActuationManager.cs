using System.Collections;

using UnityEngine;

public class BossActuationManager : MonoBehaviour, IActuate
{
    [SerializeField] BossConfig bossConfig;

    private GameObject bossPrefab;
    private BossController bossController;
    private GeneralResources generalResources;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        generalResources = FindObjectOfType<GeneralResources>();
    }

    public void Actuate(IConfiguration configuration = null)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(ActuateCoroutine());
        }
    }

    private void ResolveDependencies()
    {
        bossPrefab = bossConfig.GetBossPrefab();
    }

    private IEnumerator ActuateCoroutine()
    {
        ResolveDependencies();

        GameObject prefab = Instantiate(bossPrefab, new Vector3(-1.0f, -1.0f, 1.0f), Quaternion.identity) as GameObject;
        prefab.transform.parent = generalResources.GetActuatorFolder().transform;

        bossController = prefab.GetComponent<BossController>() as BossController;
        Vector3 spawnPoint = bossController.GetSpawnPoint();
        prefab.transform.position = spawnPoint;

        bossController.Actuate();

        yield return null;
    }
}