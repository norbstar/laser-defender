using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class LevelOld : ScriptableObject
{
    [Header("Backdrop")]
    [SerializeField] SpriteAssetPack backdropAssetPack;

    [Header("Background")]
    [SerializeField] LayerPack backgroundLayerPack;

    [Header("Gameplay")]
    [SerializeField] LayerPack gameplayLayerPack;

    [Header("Foreground")]
    [SerializeField] LayerPack foregroundLayerPack;

    [Header("Waves")]
    [SerializeField] WaveConfigPack waveConfigPack;

    [Header("Audio")]
    [SerializeField] AudioClip audioClip;

    [Header("Exposition")]
    [SerializeField] TextPack expositionPack;

    public SpriteAssetPack GetBackdropAssetPack()
    {
        return backdropAssetPack;
    }

    public LayerPack GetGameplayLayerPack()
    {
        return gameplayLayerPack;
    }

    public LayerPack GetBackgroundLayerPack()
    {
        return backgroundLayerPack;
    }

    public LayerPack GetForegroundLayerPack()
    {
        return foregroundLayerPack;
    }

    public WaveConfigPack GetWaveConfigPack()
    {
        return waveConfigPack;
    }

    public AudioClip GetAudioClip()
    {
        return audioClip;
    }
    public TextPack GetExpositionPack()
    {
        return expositionPack;
    }
}