using UnityEngine;

public class MissileManager : MonoBehaviour
{
    [SerializeField] GameObject[] missiles;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject missile in missiles)
        {
            IActuate missileController = missile.GetComponent<MissileController>() as MissileController;

            if (missileController != null)
            {
                missileController?.Actuate(new MissileController.Configuration
                {
                    Target = GameObject.FindGameObjectWithTag("Turret")
                });
            }
        }
    }
}