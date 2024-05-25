using System.Collections;

using UnityEngine;

public class TransformLerpOverTime : AbstractTransformBehaviour, IActuate, IAcutationTest
{
    [Range(0.0f, 10.0f)]
    [SerializeField] float duration = 1.0f;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration)
    {
        StartCoroutine(Co_Transform());
    }

    private IEnumerator Co_Transform()
    {
        transform.localPosition = originPosition;
        transform.localRotation = Quaternion.Euler(originRotation);
        transform.localScale = originScale;

        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) / duration;

            if (position.enable)
            {
                transform.localPosition = Vector3.Lerp(originPosition, position.target, fractionComplete);
            }

            if (rotation.enable)
            {
                transform.localRotation = Quaternion.Slerp(Quaternion.Euler(originRotation), Quaternion.Euler(rotation.target), fractionComplete);
            }

            if (scale.enable)
            {
                transform.localScale = Vector2.Lerp(originScale, scale.target, fractionComplete);
            }

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnComplete();
            }

            yield return null;
        }
    }

    private void OnComplete()
    {
        Debug.Log($"OnComplete");
    }
}