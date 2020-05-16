using UnityEngine;

[CreateAssetMenu(menuName = "Prefab Keyframe Event")]
public class PrefabKeyframeEvent : KeyframeEvent
{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector3 position = Vector3.zero;
    [SerializeField] Vector3 rotation = Vector3.zero;
    [SerializeField] Vector3 scale = Vector3.one;
    [SerializeField] float destroyAfter;
}