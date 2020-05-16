using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cube))]
public class CubeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Generate"))
        {
            //var cube = Selection.activeGameObject.GetComponent<Cube>() as Cube;
            var cube = (Cube) target;

            cube.GenerateColor();
        }

        if (GUILayout.Button("Reset"))
        {
            //var cube = Selection.activeGameObject.GetComponent<Cube>() as Cube;
            var cube = (Cube) target;

            cube.ResetColor();
        }

        GUILayout.EndHorizontal();
    }
}