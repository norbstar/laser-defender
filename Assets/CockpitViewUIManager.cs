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
        StartCoroutine(ShowCockpitCoroutine());
    }

    private IEnumerator ShowCockpitCoroutine()
    {
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) * speed;
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0.0f, 1.0f, (float) fractionComplete));

            complete = (fractionComplete >= 1.0f);

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
        StartCoroutine(HideCockpitCoroutine());
    }

    private IEnumerator HideCockpitCoroutine()
    {
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) * speed;
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1.0f, 0.0f, (float) fractionComplete));

            complete = (fractionComplete >= 1.0f);

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