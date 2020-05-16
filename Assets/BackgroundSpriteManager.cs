using UnityEngine;

public class BackgroundSpriteManager : MonoBehaviour
{
    [SerializeField] Color color;

    private new SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        ResolveComponents();

        renderer.material.SetColor("_Color", color);
    }

    private void ResolveComponents()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
}