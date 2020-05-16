using UnityEngine;

public class ScheduledEventActuator : MonoBehaviour
{
    [SerializeField] KeyframeSequence keyframeSequence;

    public KeyframeSequence GetKeyframeSequence()
    {
        return keyframeSequence;
    }
}