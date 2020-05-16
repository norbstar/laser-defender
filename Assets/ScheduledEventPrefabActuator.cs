using UnityEngine;

public class ScheduledEventPrefabActuator : MonoBehaviour
{
    [SerializeField] PrefabKeyframeSequence keyframeSequence;

    public PrefabKeyframeSequence GetKeyframeSequence()
    {
        return keyframeSequence;
    }
}