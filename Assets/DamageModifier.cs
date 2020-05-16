using UnityEngine;

public class DamageModifier : MonoBehaviour
{
    private AsteroidLayerManager asteroidLayerManager;
    private bool enabled = true;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        asteroidLayerManager = GetComponent<AsteroidLayerManager>() as AsteroidLayerManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enabled = !enabled;

            //BroadcastMessage("EnableHealth", enabled);

            //var asteroidControllers = GameObject.FindObjectsOfType<AsteroidController>() as AsteroidController[];

            //if (asteroidControllers != null)
            //{
            //    foreach (AsteroidController asteroidController in asteroidControllers)
            //    {
            //        DamageManager.SetState(asteroidController.gameObject, enabled);
            //    }
            //}

            if (asteroidLayerManager != null)
            {
                asteroidLayerManager.EnableCollisions(enabled);
            }
        }
    }
}