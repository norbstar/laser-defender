namespace UnityEditor
{
    [CustomEditor(typeof(MinMaxRange))]

    public class MinMaxSliderEditor : Editor
    {
        //public class MinMaxSettings
        //{
        //    public float MinValue { get; set; } = 5;
        //    public float MinLimit { get; set; } = 0;
        //    public float MaxValue { get; set; } = 10;
        //    public float MaxLimit { get; set; } = 15;
        //}

        private float minVal = 5;
        private float minLimit = 0;
        private float maxVal = 10;
        private float maxLimit = 15;

        //private MinMaxSettings settings = new MinMaxSettings();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //EditorGUILayout.LabelField("Min:", minVal.ToString());
            minVal = EditorGUILayout.FloatField("Min:", minVal);
            //EditorGUILayout.LabelField("Max:", maxVal.ToString());
            maxVal = EditorGUILayout.FloatField("Max:", maxVal);
            EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, minLimit, maxLimit);

            //float minValue = settings.MinValue;
            //float minLimit = settings.MinLimit;
            //float maxValue = settings.MaxValue;
            //float maxLimit = settings.MaxLimit;

            //settings.MinValue = EditorGUILayout.FloatField("Min:", settings.MinValue);
            //settings.MaxValue = EditorGUILayout.FloatField("Max:", settings.MaxValue);
            //EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, minLimit, maxLimit);
        }
    }
}