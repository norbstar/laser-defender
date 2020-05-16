using UnityEngine;

public class TurretManager : MonoBehaviour
{
    [SerializeField] GameObject[] turrets;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject turret in turrets)
        {
            IActuate turretController = turret.GetComponent<TurretControllerOriginal>() as TurretControllerOriginal;

            if (turretController != null)
            {
                turretController?.Actuate(new TurretControllerOriginal.Configuration
                {
                    Target = GameObject.FindGameObjectWithTag("Player")
                });
            }
            else
            {
                turretController = turret.GetComponent<SmartTurretController>() as SmartTurretController;

                if (turretController != null)
                {
                    turretController?.Actuate();
                }
            }
        }
    }
}