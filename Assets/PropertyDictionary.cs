using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Property Dictionary")]
public class PropertyDictionary : ScriptableObject
{
    [Serializable]
    public class KeyValue
    {
        public string key;
        public string value;
    }

    [SerializeField] KeyValue[] properties;
}