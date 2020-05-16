using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [SerializeField] float speed = 1.0f;

    private new SpriteRenderer renderer;
    //private Color[] colors;
    private int colorIndex;
    private float startTime;
    private Color originColor, targetColor;

    // Start is called before the first frame update
    void Start()
    {
        ResolveComponents();

        //colors = new Color[] { Color.white, Color.cyan };
        colorIndex = 0;

        QueueNextTransform();
    }

    private void ResolveComponents()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

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
        double fractionComplete = (Time.time - startTime) * speed;
        renderer.material.SetColor("_Color", Color.Lerp(originColor, targetColor, (float)fractionComplete));

        if (fractionComplete > 1.0f)
        {
            QueueNextTransform();
        }
    }
}