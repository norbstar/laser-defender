using UnityEngine;

public class KeyframeSequenceActuationManagerDemo : MonoBehaviour
{
    [SerializeField] KeyframeSequenceActuationManager keyframeSequenceActuationManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            keyframeSequenceActuationManager.Actuate();
        }
    }
}