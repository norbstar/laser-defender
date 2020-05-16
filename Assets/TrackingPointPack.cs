using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Tracking Point Pack")]
public class TrackingPointPack : ScriptableObject
{
    [Serializable]
    public class TrackingPointGroup
    {
        public int id;
        public TrackingPoint[] trackingPoints;
    }

    [Serializable]
    public class TrackingPoint
    {
        public GameObject prefab;
        public Vector3 position = Vector3.zero;
        public Vector3 scale = Vector3.one;
        public Vector3 rotation = Vector3.zero;
    }

    [SerializeField] TrackingPointGroup[] groups;

    public TrackingPointGroup[] GetTrackingPointGroups()
    {
        return groups;
    }
}