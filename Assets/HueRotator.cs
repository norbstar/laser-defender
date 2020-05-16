using System.Collections;

using UnityEngine;

public class HueRotator : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] float duration = 10.0f;
    public enum Direction
    {
        FORWARD,
        REVERSE
    }

    [SerializeField] Direction direction;

    private SpriteRenderer renderer;

    void Awake()
    {
        ResolveComponents();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(ColorShiftCoroutine());
    }

    private void ResolveComponents()
    {
        renderer = GetComponent<SpriteRenderer>() as SpriteRenderer;
    }

    private IEnumerator ColorShiftCoroutine()
    {
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / duration;

            if (direction == Direction.FORWARD)
            {
                renderer.color = gradient.Evaluate(fractionComplete);
            }
            else if (direction == Direction.REVERSE)
            {
                renderer.color = gradient.Evaluate(1.0f - fractionComplete);
            }

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnColorShiftComplete();
            }

            yield return null;
        }
    }

    private void OnColorShiftComplete()
    {
        StartCoroutine(ColorShiftCoroutine());
    }
}