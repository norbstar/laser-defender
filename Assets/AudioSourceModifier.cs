using System.Collections;

using UnityEngine;

public class AudioSourceModifier : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float duration;

    private float originalVolume;

    void Awake()
    {
        ResolveComponents();

        originalVolume = audioSource.volume;
    }

    private void ResolveComponents() { }

    public void TransformVolume(float volume, float? duration = null)
    {
        StartCoroutine(Co_TransformVolume(volume, duration));
    }

    private IEnumerator Co_TransformVolume(float volume, float? duration = null)
    {
        float originVolume = audioSource.volume;
        float targetVolume = volume;
        float appliedDuration = duration.HasValue ? duration.Value : this.duration;
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) / appliedDuration;
            var value = Mathf.Lerp(originVolume, targetVolume, fractionComplete);
            audioSource.volume = value;

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnVolumeAdjusted();
            }

            yield return null;
        }
    }

    private void OnVolumeAdjusted() { }
}