using UnityEngine;
using UnityEngine.UI;

public class TextUIManager : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        text = GetComponent<Text>();
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public string GetText()
    {
        return text.text;
    }

    public void SetTextColor(Color color)
    {
        text.color = color;
    }

    public Color GetTextColor()
    {
        return text.color;
    }

    public void SetTextScale(Vector3 scale)
    {
        text.transform.localScale = scale;
    }

    public Vector3 GetTextScale()
    {
        return text.transform.localScale;
    }
}