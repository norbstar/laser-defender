using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Surface Pack")]
public class SurfacePack : ScriptableObject
{
    [Serializable]
    public class SurfaceAsset
    {
        [Header("Tracking")]
        public bool enableTracking = false;
        public TrackingPointMapPack trackingPointMapPack;
    }

    [SerializeField] SurfaceAsset pack;

    public SurfaceAsset Pack { get => pack; }
}