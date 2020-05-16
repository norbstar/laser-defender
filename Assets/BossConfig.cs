using UnityEngine;

[CreateAssetMenu(menuName = "Boss Configuration")]
public class BossConfig : ScriptableObject
{
    [SerializeField] GameObject bossPrefab;

    public GameObject GetBossPrefab()
    {
        return bossPrefab;
    }
}