using UnityEngine;

[CreateAssetMenu(menuName = "Input Keyframe Event")]
public class InputKeyframeEvent : KeyframeEvent
{
    [SerializeField] Vector2 input;
}