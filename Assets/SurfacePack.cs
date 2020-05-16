using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Surface Pack")]
public class SurfacePack : ScriptableObject
{
    [Serializable]
    public class Pack
    {
        [Header("Tracking")]
        public bool enableTracking = false;
        public TrackingPointMapPack trackingPointMapPack;
    }

    [SerializeField] Pack pack;

    public Pack GetPack()
    {
        return pack;
    }
}