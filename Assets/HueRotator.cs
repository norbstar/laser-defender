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

    private new SpriteRenderer renderer;

    void Awake()
    {
        ResolveComponents();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(Co_ColorShift());
    }

    private void ResolveComponents()
    {
        renderer = GetComponent<SpriteRenderer>() as SpriteRenderer;
    }

    private IEnumerator Co_ColorShift()
    {
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) / duration;

            if (direction == Direction.FORWARD)
            {
                renderer.color = gradient.Evaluate(fractionComplete);
            }
            else if (direction == Direction.REVERSE)
            {
                renderer.color = gradient.Evaluate(1.0f - fractionComplete);
            }

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnColorShiftComplete();
            }

            yield return null;
        }
    }

    private void OnColorShiftComplete()
    {
        StartCoroutine(Co_ColorShift());
    }
}