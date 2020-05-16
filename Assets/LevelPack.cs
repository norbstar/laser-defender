using UnityEngine;

[CreateAssetMenu(menuName = "Level Pack")]
public class LevelPack : ScriptableObject
{
    [Header("Canvas")]
    [SerializeField] SpriteAssetPack canvasAssetPack;

    [Header("Background")]
    [SerializeField] SurfacePack backgroundPack;

    [Header("Gameplay")]
    [SerializeField] GameplaySurfacePack gameplayPack;

    [Header("Foreground")]
    [SerializeField] SurfacePack foregroundPack;

    [Header("Waves")]
    [SerializeField] WaveConfigPack waveConfigPack;

    [Header("Audio")]
    [SerializeField] AudioClip audioClip;

    [Header("Exposition")]
    [SerializeField] TextPack expositionPack;

    public SpriteAssetPack GetCanvasAssetPack()
    {
        return canvasAssetPack;
    }

    public SurfacePack GetBackgroundPack()
    {
        return backgroundPack;
    }

    public GameplaySurfacePack GetGameplayPack()
    {
        return gameplayPack;
    }

    public SurfacePack GetForegroundPack()
    {
        return foregroundPack;
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