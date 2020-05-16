using UnityEngine;

public class PrefabKeyframeConfig : KeyframeConfig
{
    [Header("Prefab")]
    [SerializeField] GameObject prefab;
    [SerializeField] bool destroyPrefab = false;
    [SerializeField] float destroyPrefabAfterMs = 0.0f;

    [Header("Audio")]
    [SerializeField] AudioClip audioClip;
    [SerializeField] float audioVolume = 1.0f;
    [SerializeField] bool scaleAudioToPrefabScale = false;

    public GameObject GetPrefab()
    {
        return prefab;
    }

    public bool GetDestroyPrefab()
    {
        return destroyPrefab;
    }
    public float GetDestroyPrefabAfterMs()
    {
        return destroyPrefabAfterMs;
    }

    public AudioClip GetAudioClip()
    {
        return audioClip;
    }
    
    public float GetAudioVolume()
    {
        return audioVolume;
    }

    public bool GetScaleAudioToPrefabScale()
    {
        return scaleAudioToPrefabScale;
    }
}