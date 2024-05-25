using UnityEngine;

public class BackgroundSpriteManager : MonoBehaviour
{
    [SerializeField] Color color;

    private new SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        renderer.material.SetColor("_Color", color);
    }
}