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
            StartCoroutine(Co_Actuate());
        }
    }

    private void ResolveDependencies()
    {
        bossPrefab = bossConfig.GetBossPrefab();
    }

    private IEnumerator Co_Actuate()
    {
        ResolveDependencies();

        GameObject prefab = Instantiate(bossPrefab, new Vector3(-1.0f, -1.0f, 1.0f), Quaternion.identity) as GameObject;
        prefab.transform.parent = generalResources.ActuatorFolder.transform;

        bossController = prefab.GetComponent<BossController>() as BossController;
        Vector3 spawnPoint = bossController.GetSpawnPoint();
        prefab.transform.position = spawnPoint;

        bossController.Actuate();

        yield return null;
    }
}