using UnityEngine;

public class BossActuator : MonoBehaviour {
    [SerializeField] BossConfig bossConfig;

    public BossConfig GetBossConfig()
    {
        return bossConfig;
    }
}