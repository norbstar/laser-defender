using UnityEngine;

public abstract class GUIMonoBehaviour : MonoBehaviour
{
    protected GUIStyle guiStyle;

    public abstract void SetGUIAttributes(GUIStyle guiStyle);

    protected void ConfigGUIStyle(string fontName = "Block Stock")
    {
        Font myFont = (Font) Resources.Load($"Fonts/{fontName}", typeof(Font));

        guiStyle = new GUIStyle
        {
            font = myFont,
            fontStyle = FontStyle.Normal,
            fontSize = 24
        };

        guiStyle.normal.textColor = Color.white;
    }

    public GUIStyle GetGUIStyle()
    {
        return guiStyle;
    }

    void OnGUI()
    {
        SetGUIAttributes(guiStyle);
    }
}