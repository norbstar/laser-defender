using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class LevelOld : ScriptableObject
{
    [Header("Backdrop")]
    [SerializeField] SpriteAssetPack spritePack;

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

    public SpriteAssetPack SpritePack { get => spritePack; }

    public LayerPack GameplayLayerPack { get => gameplayLayerPack; }

    public LayerPack BackgroundLayerPack { get => backgroundLayerPack; }

    public LayerPack ForegroundLayerPack { get => foregroundLayerPack; }

    public WaveConfigPack WaveConfigPack { get => waveConfigPack; }

    public AudioClip AudioClip { get => audioClip; }

    public TextPack ExpositionPack { get => expositionPack; }
}