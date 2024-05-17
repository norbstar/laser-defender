using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Layer Pack")]
public class LayerPack : ScriptableObject
{
    [Serializable]
    public class Clouds
    {
        public bool enableClouds;
        public CloudPack cloudPack;
    }

    [Serializable]
    public class DynamicInjection
    {
        public Clouds clouds;
    }

    [Serializable]
    public class SpawnSettingsOverride
    {
        public bool enableOverrides = false;
        //public AsteroidLayerManager.SpawnSettings spawnSettings;
    }

    [Serializable]
    public class LayerAsset
    {
        [Header("Asteroid Layer")]
        public bool enableAsteroidLayer = false;
        public SpawnSettingsOverride overrides;

        [Header("Debris Layer")]
        public bool enableDebrisLayer = false;

        [Header("Tracking Layer")]
        public bool enableTrackingLayer = false;
        public TrackingPointMapPack trackingPointMapPack;
    }

    [SerializeField] LayerAsset pack;

    public LayerAsset Pack { get => pack; }
}