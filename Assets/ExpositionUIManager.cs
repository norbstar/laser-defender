﻿using System.Collections;

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
            StartCoroutine(ActuateCoroutine());
        }
    }

    private void ResolveDependencies() { }

    private IEnumerator ActuateCoroutine()
    {
        ResolveDependencies();

        if (textPack != null)
        {
            foreach (TextPack.TextAsset textAsset in textPack.GetPack())
            {
                string text = textAsset.text;
                textUIManager.SetText(text);

                float textScale = textAsset.textScale;
                textUIManager.SetTextScale(Vector3.one * textScale);

                Color textColor = textAsset.textColor;
                textUIManager.SetTextColor(textColor);

                yield return StartCoroutine(FadeInText());
                yield return new WaitForSeconds(promptDelay);
                yield return StartCoroutine(FadeOutText());
                yield return new WaitForSeconds(interPromptDelay);
            }

            textUIManager.SetText("");
            onExpositionCompleteDelegate?.Invoke();
        }
    }

    private IEnumerator FadeInText()
    {
        Color originalColor = textUIManager.GetTextColor();
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
        Color originalColor = textUIManager.GetTextColor();
        float startTime = Time.time;
        float duration = transitionDelay;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / duration;

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