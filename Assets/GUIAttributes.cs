using System.Collections.Generic;

using UnityEngine;

public class GUIAttributes : GUIMonoBehaviour
{
    private IDictionary<string, string> attributes;

    void Awake()
    {
        ConfigGUIStyle();

        guiStyle.fontSize = 8;
        guiStyle.font = (Font) Resources.Load("Fonts/Block Stock");
        guiStyle.normal.textColor = Color.white;

        attributes = new Dictionary<string, string>();
    }

    public void SetAttribute(string key, string attribute)
    {
        bool addAttribute = true;

        if (attributes.ContainsKey(key))
        {
            if (attributes[key].Equals(attribute))
            {
                addAttribute = false;
            }
            else
            {
                attributes.Remove(key);
            }
        }

        if (addAttribute)
        {
            attributes.Add(key, attribute);
        }
    }

    public void UnsetAttribute(string key)
    {
        if (attributes.ContainsKey(key))
        {
            attributes.Remove(key);
        }
    }

    public override void SetGUIAttributes(GUIStyle guiStyle)
    {
        float y = 25;

        foreach (KeyValuePair<string, string> keyValuePair in attributes)
        {
            string attribute = keyValuePair.Value;

            GUI.Label(new Rect(20, y, 200, 40), $"{attribute}", guiStyle);
            y += 25;
        }
    }
}