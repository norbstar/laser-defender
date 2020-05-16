using UnityEngine;
using UnityEngine.UI;

public class HealthBarSliderUIManager : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;

    private Slider slider;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        slider = GetComponent<Slider>() as Slider;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public float GetHealth()
    {
        return slider.value;
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = slider.maxValue;
        fill.color = gradient.Evaluate(1.0f);
    }

    public float GetMinHealth()
    {
        return slider.minValue;
    }
    
    public float GetMaxHealth()
    {
        return slider.maxValue;
    }
}