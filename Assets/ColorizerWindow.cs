using UnityEngine;
using UnityEditor;

public class ColorizerWindow : EditorWindow
{
    [SerializeField] Color color;

    [MenuItem("Window/Colorizer")]
    public static void ShowWindow()
    {
        GetWindow<ColorizerWindow>("Colorizer");
    }

    void OnGUI()
    {
        GUILayout.Label("Color the selected objects!", EditorStyles.boldLabel);
        color = EditorGUILayout.ColorField("Color", color);

        if (GUILayout.Button("Colorize"))
        {
            Colorize();
        }
    }

    private void Colorize()
    {
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            var renderer = gameObject.GetComponent<Renderer>() as Renderer;

            if (renderer != null)
            {
                renderer.sharedMaterial.color = color;
            }
        }
    }
}