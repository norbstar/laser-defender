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
        GameObject prefab = Instantiate(asteroidLayerPrefab, transform.position + new Vector3(0.0f, 0.0f, -0.1f), Quaternion.identity) as GameObject;
        prefab.transform.parent = transform;
    }

    public void InstantiateTrackingLayer(TrackingPointMapPack trackingPointMapPack)
    {
        GameObject prefab = Instantiate(trackingLayerPrefab, transform.position + new Vector3(0.0f, 8.0f, -0.2f), Quaternion.identity) as GameObject;
        prefab.transform.parent = transform;

        trackingLayerManager = prefab.GetComponent<TrackingLayerManager>();
        trackingLayerManager.Initiate(trackingPointMapPack, GetScrollSpeed(), indicatorColor);
    }

    private void OnUpdateScrollSpeed(float scrollSpeed) => trackingLayerManager?.SetScrollSpeed(scrollSpeed);
}