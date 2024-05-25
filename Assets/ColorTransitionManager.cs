using UnityEngine;

public class ColorTransitionManager : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] float speed = 1.0f;

    private new SpriteRenderer renderer;
    private int colorIndex;
    private Color originColor;
    private Color targetColor;
    private float startTime;

    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        colorIndex = 0;
    }

    // Start is called before the first frame update
    void Start() => QueueNextTransform();

    private void QueueNextTransform()
    {
        startTime = Time.time;
        originColor = colors[colorIndex];

        if (colorIndex + 1 > colors.Length - 1)
        {
            colorIndex = 0;
            targetColor = colors[colorIndex];
        }
        else
        {
            ++colorIndex;
            targetColor = colors[colorIndex];
        }
    }

    // Update is called once per frame
    void Update()
    {
        var fractionComplete = (Time.time - startTime) * speed;
        renderer.material.SetColor("_Color", Color.Lerp(originColor, targetColor, fractionComplete));

        if (fractionComplete > 1.0f)
        {
            QueueNextTransform();
        }
    }
}