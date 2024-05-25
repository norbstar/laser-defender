using System.Collections;

using UnityEngine;

public class ExpositionUIManager : MonoBehaviour, IActuate
{
    public delegate void OnExpositionComplete();

    public class Configuration : GameplayConfiguration
    {
        public TextPack TextPack { get; set; }
    }

    [SerializeField] TextUIManager textUIManager;
    [SerializeField] float promptDelay = 0.5f;
    [SerializeField] float transitionDelay = 0.5f;
    [SerializeField] float interPromptDelay = 0.5f;

    private OnExpositionComplete onExpositionCompleteDelegate;
    private TextPack textPack;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents() { }

    public void RegisterDelegate(OnExpositionComplete onExpositionCompleteDelegate)
    {
        this.onExpositionCompleteDelegate = onExpositionCompleteDelegate;
    }

    public void Actuate(IConfiguration configuration)
    {
        if (typeof(Configuration).IsInstanceOfType(configuration))
        {
            textPack = ((Configuration) configuration).TextPack;
        }

        if (gameObject.activeSelf)
        {
            StartCoroutine(Co_Actuate());
        }
    }

    private void ResolveDependencies() { }

    private IEnumerator Co_Actuate()
    {
        ResolveDependencies();

        if (textPack != null)
        {
            foreach (TextPack.TextAsset textAsset in textPack.Pack)
            {
                string text = textAsset.text;
                textUIManager.SetText(text);

                float textScale = textAsset.textScale;
                textUIManager.SetTextScale(Vector3.one * textScale);

                Color textColor = textAsset.textColor;
                textUIManager.SetTextColor(textColor);

                yield return StartCoroutine(Co_FadeInText());
                yield return new WaitForSeconds(promptDelay);
                yield return StartCoroutine(Co_FadeOutText());
                yield return new WaitForSeconds(interPromptDelay);
            }

            textUIManager.SetText("");
            onExpositionCompleteDelegate?.Invoke();
        }
    }

    private IEnumerator Co_FadeInText()
    {
        Color originalColor = textUIManager.GetTextColor();
        float startTime = Time.time;
        float duration = transitionDelay;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) / duration;

            textUIManager.SetTextColor(new Color(originalColor.r, originalColor.g, originalColor.b, fractionComplete));

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnFadeInComplete();
            }

            yield return null;
        }
    }

    private IEnumerator Co_FadeOutText()
    {
        Color originalColor = textUIManager.GetTextColor();
        float startTime = Time.time;
        float duration = transitionDelay;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) / duration;

            textUIManager.SetTextColor(new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f - fractionComplete));

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnFadeOutComplete();
            }

            yield return null;
        }
    }

    private void OnFadeInComplete() { }

    private void OnFadeOutComplete() { }
}