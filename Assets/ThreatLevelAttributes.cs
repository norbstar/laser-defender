using UnityEngine;

public class ThreatLevelAttributes : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] int threatLevel;

    public float ThreatLevel => threatLevel;
}