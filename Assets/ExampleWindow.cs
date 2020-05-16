using UnityEngine;
using UnityEditor;

public class ExampleWindow : EditorWindow
{
    private string myString = "Hello World";

    [MenuItem("Window/Example")]
    public static void ShowWindow()
    {
        GetWindow<ExampleWindow>("Example");
    }

    void OnGUI()
    {
        GUILayout.Label("Label", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Name", myString, EditorStyles.label);

        if (GUILayout.Button("Click Me"))
        {
            Debug.Log($"You clicked me!");
        }
    }
}