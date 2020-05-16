using UnityEngine;

public class AutoActuator : MonoBehaviour
{
    [SerializeField] IActuate actuatorScript;

    // Start is called before the first frame update
    void Start()
    {
        actuatorScript?.Actuate();
    }
}