using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Tracking Point Map Pack")]
public class TrackingPointMapPack : ScriptableObject
{
    [Serializable]
    public class Map
    {
        public int id;
        public GameObject prefab;
    }

    [Serializable]
    public class Pack
    {
        public Map[] maps;
    }

    [SerializeField] Pack pack;

    public Pack GetPack()
    {
        return pack;
    }
}