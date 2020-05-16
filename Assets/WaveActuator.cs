using UnityEngine;

public class WaveActuator : MonoBehaviour
{
    [SerializeField] WaveConfig waveConfig;

    public WaveConfig GetWaveConfig()
    {
        return waveConfig;
    }
}