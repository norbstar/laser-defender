using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class CockpitViewUIManager : MonoBehaviour
{
    public delegate void OnCockpitShown();
    public delegate void OnCockpitHidden();

    public class Delegates
    {
        public OnCockpitShown OnCockpitShownDelegate { get; set; }
        public OnCockpitHidden OnCockpitHiddenDelegate { get; set; }
    }

    [SerializeField] float speed = 1.0f;

    private Delegates delegates;
    private Image image;
    private Color originalColor;

    void Awake()
    {
        ResolveComponents();

        originalColor = image.color;
    }

    private void ResolveComponents()
    {
        image = GetComponent<Image>();
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    public void SetAsset(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void ShowCockpit()
    {
        StartCoroutine(Co_ShowCockpit());
    }

    private IEnumerator Co_ShowCockpit()
    {
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * speed;
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0f, 1f, fractionComplete));

            complete = fractionComplete >= 1f;

            if (complete)
            {
                delegates?.OnCockpitShownDelegate?.Invoke();

                OnComplete();
            }

            yield return null;
        }
    }

    public void HideCockpit()
    {
        StartCoroutine(Co_HideCockpit());
    }

    private IEnumerator Co_HideCockpit()
    {
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * speed;
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, fractionComplete));

            complete = fractionComplete >= 1f;

            if (complete)
            {
                delegates?.OnCockpitHiddenDelegate?.Invoke();

                OnComplete();
            }

            yield return null;
        }
    }

    private void OnComplete() { }
}