using UnityEngine;

[CreateAssetMenu(menuName = "Keyframe Sequence")]
public class KeyframeSequence : ScriptableObject
{
    [SerializeField] string identifier;
    [SerializeField] KeyframeEvent[] events;
}