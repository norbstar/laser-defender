using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Surface Layer Pack")]
public class SurfaceLayerPack : ScriptableObject
{
    [Serializable]
    public class Pack
    {
        [Header("Tracking")]
        public bool enableTracking = false;
        public RenderLayer layer;
        public TrackingPointMapPack[] trackingPointMapPacks;
    }

    [SerializeField] Pack pack;

    public Pack GetPack()
    {
        return pack;
    }
}