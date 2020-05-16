using UnityEngine;

[CreateAssetMenu(menuName = "Wave Config Pack")]
public class WaveConfigPack : ScriptableObject
{
    [Header("Configuration")]
    [SerializeField] WaveConfig[] pack;

    public WaveConfig[] GetPack()
    {
        return pack;
    }
}