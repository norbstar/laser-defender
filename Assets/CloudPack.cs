using UnityEngine;

[CreateAssetMenu(menuName = "Cloud Pack")]
public class CloudPack : ScriptableObject
{
    [SerializeField] GameObject[] pack;

    public GameObject[] GetPack()
    {
        return pack;
    }
}