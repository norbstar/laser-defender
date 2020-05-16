using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Surface Pack")]
public class GameplaySurfacePack : ScriptableObject
{
    [Serializable]
    public class Pack
    {
        [Header("Tracking")]
        public bool enableTracking = false;
        public TrackingPointMapPack subSurfaceTrackingPointMapPack;
        public TrackingPointMapPack surfaceTrackingPointMapPack;
    }

    [SerializeField] Pack pack;

    public Pack GetPack()
    {
        return pack;
    }
}