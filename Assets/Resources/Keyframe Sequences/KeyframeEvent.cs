using UnityEngine;

[CreateAssetMenu(menuName = "Keyframe Event")]
public class KeyframeEvent : ScriptableObject
{
    [SerializeField] float keyframe;
    [SerializeField] PropertyDictionary propertyDictionary;
}