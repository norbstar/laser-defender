using UnityEngine;

[CreateAssetMenu(menuName = "Level Pack")]
public class LevelPack : ScriptableObject
{
    [Header("Canvas")]
    [SerializeField] SpriteAssetPack spritePack;

    [Header("Background")]
    [SerializeField] SurfacePack backgroundPack;

    [Header("Gameplay")]
    [SerializeField] SurfacePack gameplayPack;

    [Header("Foreground")]
    [SerializeField] SurfacePack foregroundPack;

    [Header("Waves")]
    [SerializeField] WaveConfigPack waveConfigPack;

    [Header("Audio")]
    [SerializeField] AudioClip audioClip;

    [Header("Exposition")]
    [SerializeField] TextPack expositionPack;

    public SpriteAssetPack SpritePack { get => spritePack; }

    public SurfacePack BackgroundPack { get => backgroundPack; }

    public SurfacePack GameplayPack { get => gameplayPack; }

    public SurfacePack ForegroundPack { get => foregroundPack; }

    public WaveConfigPack WaveConfigPack { get => waveConfigPack; }

    public AudioClip AudioClip { get => audioClip; }

    public TextPack ExpositionPack { get => expositionPack; }
}