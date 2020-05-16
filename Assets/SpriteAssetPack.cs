using UnityEngine;

[CreateAssetMenu(menuName = "Sprite Asset Pack")]
public class SpriteAssetPack : ScriptableObject
{
    [SerializeField] Sprite[] pack;

    public Sprite[] GetPack()
    {
        return pack;
    }
}