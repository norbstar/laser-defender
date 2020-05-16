using System;

using UnityEngine;

public class KeyframeConfig : MonoBehaviour
{
    [SerializeField] float keyframe;

    [Serializable]
    public class KeyValue
    {
        public string key;
        public string value;
    }

    [SerializeField] KeyValue[] dictionary;

    public float GetKeyframe()
    {
        return keyframe;
    }

    public KeyValue[] GetDictionary()
    {
        return dictionary;
    }
}