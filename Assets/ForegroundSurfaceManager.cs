using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ForegroundSurfaceManager : SurfaceManager, IActuate
{
    public class Configuration : GameplayConfiguration
    {
        public SurfacePack SurfacePack { get; set; }
    }

    private TrackingPointMapPack trackingPointMapPack;
    private Vector3 originPosition;
    private int index;
    private SortedList<float, GameObject> actuators;
    private int mainCanvasId, bufferCanvasId;
    private GameObject main, mainIdTag, buffer, bufferIdTag;
    private GameObject map;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();

        index = 0;
        mainCanvasId = 0;
        bufferCanvasId = mainCanvasId + 1;
        originPosition = transform.position;
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration)
    {
        SurfacePack surfacePack = null;

        if (typeof(Configuration).IsInstanceOfType(configuration))
        {
            surfacePack = ((Configuration) configuration).SurfacePack;
        }

        SurfacePack.SurfaceAsset pack = surfacePack.Pack;

        if (pack.enableTracking)
        {
            trackingPointMapPack = pack.trackingPointMapPack;
        }

        StartCoroutine(Co_Actuate());
    }

    private void ResolveDependencies(GameObject prefab)
    {
        main = prefab.transform.Find("Main").gameObject as GameObject;
        mainIdTag = prefab.transform.Find("Main Id Tag").gameObject as GameObject;

        buffer = prefab.transform.Find("Buffer").gameObject as GameObject;
        bufferIdTag = prefab.transform.Find("Buffer Id Tag").gameObject as GameObject;
    }

    private IEnumerator Co_Actuate()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GameObject prefab = Instantiate(surfaceLayerPrefab, transform.position, Quaternion.identity) as GameObject;
        prefab.transform.parent = transform;
        prefab.layer = (int) RenderLayer.FOREGROUND;
        prefab.name = "Foreground Surface Layer";

        //var surfaceLayerManager = prefab.GetComponent<SurfaceLayerManager>() as SurfaceLayerManager;
        //surfaceLayerManager.Actuate();

        ResolveDependencies(prefab);

        TrackingPointMapPack.Map trackingPointMap = GetTrackingPointMap(mainCanvasId);

        if (trackingPointMap != null)
        {
            GameObject trackingPointMapPrefab = trackingPointMap.prefab;

            if (trackingPointMapPrefab != null)
            {
                Vector3 position = main.transform.position;
                map = Instantiate(trackingPointMapPrefab, position, Quaternion.identity) as GameObject;
                map.transform.parent = main.transform;
            }
        }

        StartCoroutine(Co_Scroll());

        yield return null;
    }

    private TrackingPointMapPack.Map GetTrackingPointMap(int id)
    {
        TrackingPointMapPack.Pack pack = trackingPointMapPack.GetPack();

        foreach (TrackingPointMapPack.Map map in pack.maps)
        {
            if (map.id == id)
            {
                return map;
            }
        }

        return null;
    }

    private IEnumerator Co_Scroll()
    {
        UpdateIdentifiers();

        Vector3 targetPosition = new Vector3(0f, transform.position.y - InGameManager.ScreenRatio.y, transform.position.z);
        float journeyLength = InGameManager.ScreenRatio.y;
        float speedAdjustedDeltaTime = 0f;
        bool complete = false;

        if (trackingPointMapPack != null)
        {
            TrackingPointMapPack.Map trackingPointMap = GetTrackingPointMap(bufferCanvasId);

            if (trackingPointMap != null)
            {
                GameObject trackingPointMapPrefab = trackingPointMap.prefab;

                if (trackingPointMapPrefab != null)
                {
                    Vector3 position = buffer.transform.position;
                    map = Instantiate(trackingPointMapPrefab, position, Quaternion.identity) as GameObject;
                    map.transform.parent = buffer.transform;

                    //AssignLayerToGroup(map.transform, Layer.FOREGROUND);

                    actuators = CaptureActuators();
                }
            }
        }

        while (!complete)
        {
            var fractionComplete = speedAdjustedDeltaTime / journeyLength;

            transform.position = Vector3.Lerp(originPosition, targetPosition, (float)fractionComplete);

            StartCoroutine(Co_ActionActuators());

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnComplete();
            }

            speedAdjustedDeltaTime += scrollSpeed * Time.deltaTime;
            // Debug.Log($"Scroll Speed: {scrollSpeed} Speed Adjusted Delta Time: {speedAdjustedDeltaTime}");

            yield return null;
        }
    }

    private void AssignLayerToGroup(Transform transform, RenderLayer layer)
    {
        foreach (Transform childTransform in transform)
        {
            childTransform.gameObject.layer = (int)layer;

            AssignLayerToGroup(childTransform, layer);
        }
    }

    private void UpdateIdentifiers()
    {
        var textMesh = mainIdTag.GetComponent<TextMesh>();
        textMesh.color = idTagColor;
        textMesh.text = mainCanvasId.ToString();

        textMesh = bufferIdTag.GetComponent<TextMesh>();
        textMesh.color = idTagColor;
        textMesh.text = bufferCanvasId.ToString();
    }

    private class DuplicateKeyComparer<T> : IComparer<T> where T : IComparable
    {
        #region IComparer<T> Members

        public int Compare(T keyA, T keyB)
        {
            return (keyA.CompareTo(keyB) == 0) ? 1 : keyA.CompareTo(keyB);
        }

        #endregion
    }

    private SortedList<float, GameObject> CaptureActuators()
    {
        var actuators = new SortedList<float, GameObject>(new DuplicateKeyComparer<float>());

        foreach (Transform childTransform in map.transform)
        {
            GameObject gameObject = childTransform.gameObject;

            if (childTransform.gameObject.activeSelf)
            {
                var actuation = childTransform.gameObject.GetComponent<IActuate>() as IActuate;

                if (actuation != null)
                {
                    GeometryFunctions.Bounds bounds = GeometryFunctions.GetAggregateBounds(gameObject);

                    if (bounds != null)
                    {
                        // Use the bottom most bound of the game object
                        actuators.Add(bounds.Bottom, childTransform.gameObject);
                    }
                    else
                    {
                        // Use the game objects position as there is no renderer to guage it's bottom most bounds
                        actuators.Add(childTransform.localPosition.y, childTransform.gameObject);
                    }
                }
                else
                {
                    // Add a layer actuator to the game object
                    childTransform.gameObject.AddComponent<LayerActuator>();
                    actuators.Add(0.0f, childTransform.gameObject);
                }
            }
        }

        return actuators;
    }

    private IEnumerator Co_ActionActuators()
    {
        while (actuators.Count > 0)
        {
            float key = actuators.Keys[0];
            GameObject gameObject = actuators.Values[0];

            if (gameObject == null)
            {
                actuators.RemoveAt(0);
                continue;
            }

            float target = InGameManager.ScreenRatio.y - key;
            float delta = InGameManager.ScreenRatio.y + this.gameObject.transform.position.y - target;

            if (delta <= 0)
            {
                actuators.RemoveAt(0);

                var actuation = gameObject.GetComponent<IActuate>() as IActuate;

                if ((actuation != null) && (gameObject.activeSelf))
                {
                    actuation.Actuate(new GameplayConfiguration
                    {
                        Layer = RenderLayer.FOREGROUND
                    });
                }
            }
            else
            {
                break;
            }

            yield return null;
        }
    }

    private void OnComplete()
    {
        foreach (Transform childTransform in main.transform)
        {
            Destroy(childTransform.gameObject);
        }

        for (int itr = 0; itr < buffer.transform.childCount; ++itr)
        {
            Transform childTransform = buffer.transform.GetChild(0);
            childTransform.position += new Vector3(0.0f, -InGameManager.ScreenRatio.y, 0.0f);
            childTransform.parent = main.transform;
        }

        ++index;
        bufferCanvasId = index + 1;
        transform.position = originPosition;

        StartCoroutine(Co_Scroll());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = idTagColor;
        DrawPanelGuide(transform.position);

        if ((actuators != null) && (actuators.Count > 0))
        {
            foreach (KeyValuePair<float, GameObject> actuator in actuators)
            {
                float key = actuator.Key;
                GameObject gameObject = actuator.Value;

                if (gameObject != null)
                {
                    float target = InGameManager.ScreenRatio.y - key;
                    float delta = InGameManager.ScreenRatio.y + this.gameObject.transform.position.y - target;

                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(new Vector3(gameObject.transform.position.x - 0.0625f, InGameManager.ScreenRatio.y + delta, gameObject.transform.position.z), new Vector3(gameObject.transform.position.x + 0.0625f, InGameManager.ScreenRatio.y + delta, gameObject.transform.position.z));

                    Gizmos.color = idTagColor;
                    Gizmos.DrawWireCube(gameObject.transform.position, new Vector2(0.125f, 0.125f));
                }
            }
        }
    }
}