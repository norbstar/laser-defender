using UnityEngine;

public class InputKeyframeConfig : KeyframeConfig
{
    [Header("Input")]
    [SerializeField] Vector2 input;

    public Vector2 GetInput()
    {
        return input;
    }
}