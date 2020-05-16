using UnityEngine;

public class TurretSpawnManager : MonoBehaviour
{
    [SerializeField] GameObject turretPrefab;
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 rotation;
    [SerializeField] Vector3 scale = Vector3.one;
    [SerializeField] bool actuate = true;
    [SerializeField] bool drawGizmos;

    // Start is called before the first frame update
    void Start()
    {
        var turret = Instantiate(turretPrefab, position, Quaternion.identity) as GameObject;
        turret.transform.rotation = Quaternion.Euler(rotation);
        turret.transform.localScale = scale;

        var turretController = turret.GetComponent<TurretControllerOriginal>() as TurretControllerOriginal;

        if (actuate)
        {
            turretController?.Actuate(new TurretControllerOriginal.Configuration
            {
                Target = GameObject.FindGameObjectWithTag("Player")
            });
        }
    }
}