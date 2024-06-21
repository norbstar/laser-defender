using UnityEngine;

public class LayersManager : AbstractSceneryManager
{
    [Header("Asteroid Layer")]
    [SerializeField] GameObject asteroidLayerPrefab;

    [Header("Tracking Layer")]
    [SerializeField] GameObject trackingLayerPrefab;
    [SerializeField] Color indicatorColor;

    private LayerPack.LayerAsset pack;
    private TrackingLayerManager trackingLayerManager;

    public void Initiate(LayerPack layerPack)
    {
        DestroyLayers();

        if (layerPack != null)
        {
            pack = layerPack.Pack;

            if (pack.enableAsteroidLayer)
            {
                InstantiateAsteroidLayer();
            }

            if (pack.enableTrackingLayer)
            {
                InstantiateTrackingLayer(pack.trackingPointMapPack);
            }
        }

        RegisterDelegate(OnUpdateScrollSpeed);
    }

    public void DestroyLayers()
    {
        foreach (Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);
        }
    }

    public void InstantiateAsteroidLayer()
    {
        var prefab = Instantiate(asteroidLayerPrefab, transform.position + new Vector3(0f, 0f, -0.1f), Quaternion.identity);
        prefab.transform.parent = transform;
    }

    public void InstantiateTrackingLayer(TrackingPointMapPack trackingPointMapPack)
    {
        var prefab = Instantiate(trackingLayerPrefab, transform.position + new Vector3(0f, 8f, -0.2f), Quaternion.identity);
        prefab.transform.parent = transform;

        trackingLayerManager = prefab.GetComponent<TrackingLayerManager>();
        trackingLayerManager.Initiate(gameObject.layer, trackingPointMapPack, scrollSpeed, indicatorColor);
    }

    private void OnUpdateScrollSpeed(float scrollSpeed) => trackingLayerManager?.SetScrollSpeed(scrollSpeed);
}