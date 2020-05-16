using UnityEngine;

[CreateAssetMenu(menuName = "Prefab Keyframe Sequence")]
public class PrefabKeyframeSequence : ScriptableObject
{
    [SerializeField] string identifier;
    [SerializeField] PrefabKeyframeEvent[] events;
}