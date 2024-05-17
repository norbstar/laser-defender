using UnityEngine;

namespace UnityEditor
{
    public class MinMaxSlider : EditorWindow
    {
        private float minVal = 10;
        private float minLimit = 0;
        private float maxVal = 10;
        private float maxLimit = 15;

        [MenuItem("Examples/Place Object Randomly")]
        static void Init()
        {
            MinMaxSlider window = (MinMaxSlider)GetWindow(typeof(MinMaxSlider));
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Min:", minVal.ToString());
            EditorGUILayout.LabelField("Max:", maxVal.ToString());
            EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, minLimit, maxLimit);

            if (GUILayout.Button("Move!"))
            {
                PlaceRandomly();
            }
        }

        void PlaceRandomly()
        {
            if (Selection.activeTransform)
            {
                Selection.activeTransform.position = new Vector3(Random.Range(minVal, maxVal), Random.Range(minVal, maxVal), Random.Range(minVal, maxVal));
            }
            else
            {
                Debug.LogError("Select a GameObject to randomize its position.");
            }
        }
    }
}