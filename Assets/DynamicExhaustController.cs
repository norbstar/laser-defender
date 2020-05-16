using UnityEngine;

public class DynamicExhaustController : MonoBehaviour
{
    private AudioSource audioSource;
    private float thrustRatio;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        audioSource = GetComponent<AudioSource>() as AudioSource;
    }

    public void SetThrust(float input)
    {
        // Normalize the input within the anticipated range for input (-1 to 1)
        this.thrustRatio = MathFunctions.GetRelativeRatio(-1.0f, 1.0f, input);

        SetRelativeAudioPitch();
        SetRelativeScale();
    }

    public float GetPitch()
    {
        return audioSource.pitch;
    }

    public float GetVolume()
    {
        return audioSource.volume;
    }

    public Vector3 GetScale()
    {
        return transform.localScale;
    }

    private void SetRelativeAudioPitch()
    {
        audioSource.pitch = MathFunctions.GetValueInRange(2.0f, 0.5f, thrustRatio);
        audioSource.volume = MathFunctions.GetValueInRange(0.0f, 1.0f, thrustRatio);
    }

    private void SetRelativeScale()
    {
        float scale = MathFunctions.GetValueInRange(0.0f, 2.0f, thrustRatio);
        transform.localScale = new Vector3(scale, scale, 1.0f);
    }
}