using System.Collections;

using UnityEngine;

public class CountdownUIManager : MonoBehaviour, IActuate
{
    public delegate void OnCountdownComplete();

    [SerializeField] TextUIManager textUIManager;
    [SerializeField] TextAudioPack textAudioPack;
    [SerializeField] float promptDelay = 0.25f;
    [SerializeField] float transitionDelay = 0.25f;
    [SerializeField] float interPromptDelay = 0.25f;

    private AudioSource audioSource;
    private OnCountdownComplete onCountdownCompleteDelegate;
    private Vector3 originalScale;
    private Color originalColor;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void RegisterDelegate(OnCountdownComplete onCountdownCompleteDelegate)
    {
        this.onCountdownCompleteDelegate = onCountdownCompleteDelegate;
    }

    public void Actuate(IConfiguration configuration = null)
    {
        originalScale = transform.localScale;
        originalColor = textUIManager.GetTextColor();

        if (gameObject.activeSelf)
        {
            StartCoroutine(ActuateCoroutine());
        }
    }

    private void ResolveDependencies() { }

    private IEnumerator ActuateCoroutine()
    {
        ResolveDependencies();

        foreach (TextAudioPack.TextAudioAsset textAudioAsset in textAudioPack.GetPack())
        {
            string text = textAudioAsset.text;
            textUIManager.SetText(text);

            AudioClip audioClip = textAudioAsset.audioClip;
            audioSource.PlayOneShot(audioClip);

            //yield return StartCoroutine(FadeInText());
            //yield return new WaitForSeconds(promptDelay);
            yield return StartCoroutine(FadeOutText());
            yield return new WaitForSeconds(interPromptDelay);
        }

        textUIManager.SetText("");
        onCountdownCompleteDelegate?.Invoke();
    }

    private IEnumerator FadeInText()
    {
        float startTime = Time.time;
        float duration = transitionDelay;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / duration;

            textUIManager.SetTextColor(new Color(originalColor.r, originalColor.g, originalColor.b, fractionComplete));

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnFadeInComplete();
            }

            yield return null;
        }
    }

    private IEnumerator FadeOutText()
    {
        float startTime = Time.time;
        float duration = transitionDelay;
        bool complete = false;

        textUIManager.SetTextScale(originalScale);

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / duration;

            textUIManager.SetTextScale(Vector3.one * 2.0f * fractionComplete);
            textUIManager.SetTextColor(new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f - fractionComplete));

            complete = (fractionComplete >= 1.0f);

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