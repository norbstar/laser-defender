using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SurfaceLayerManagerOld : SurfaceManager, IActuate
{
    public class Configuration : GameplayConfiguration
    {
        public SurfaceLayerPack SurfaceLayerPack { get; set; }
    }

    private IList<TrackingPointMapPack> trackingPointMapPacks;
    private Vector3 originPosition;
    private int index;
    private IDictionary<long, SortedList<float, GameObject>> actuators;
    private int mainCanvasId, bufferCanvasId;
    private GameObject main, mainIdTag, buffer, bufferIdTag;
    private IList<GameObject> maps;
    private RenderLayer layer;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();

        index = 0;
        mainCanvasId = 0;
        bufferCanvasId = mainCanvasId + 1;
        originPosition = transform.position;
        trackingPointMapPacks = new List<TrackingPointMapPack>();
        actuators = new Dictionary<long, SortedList<float, GameObject>>();
        maps = new List<GameObject>();
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration)
    {
        SurfaceLayerPack surfaceLayerPack = null;

        if (typeof(Configuration).IsInstanceOfType(configuration))
        {
            surfaceLayerPack = ((Configuration) configuration).SurfaceLayerPack;
        }

        SurfaceLayerPack.Pack pack = surfaceLayerPack.GetPack();
        layer = pack.layer;

        if (pack.enableTracking)
        {
            trackingPointMapPacks = pack.trackingPointMapPacks;
        }

        StartCoroutine(ActuateCoroutine());
    }

    private void ResolveDependencies(GameObject prefab)
    {
        main = prefab.transform.Find("Main").gameObject as GameObject;
        mainIdTag = prefab.transform.Find("Main Id Tag").gameObject as GameObject;

        buffer = prefab.transform.Find("Buffer").gameObject as GameObject;
        bufferIdTag = prefab.transform.Find("Buffer Id Tag").gameObject as GameObject;
    }

    private IEnumerator ActuateCoroutine()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (TrackingPointMapPack mapPack in trackingPointMapPacks)
        {
            GameObject prefab = Instantiate(surfaceLayerPrefab, transform.position, Quaternion.identity) as GameObject;
            prefab.transform.parent = transform;

            ResolveDependencies(prefab);

            TrackingPointMapPack.Map map = GetTrackingPointMap(mapPack, mainCanvasId);

            if (map != null)
            {
                prefab.layer = (int) layer;
                prefab.name = $"{LayerMask.LayerToName((int) layer)} Layer";

                GameObject trackingPointMapPrefab = map.prefab;

                if (trackingPointMapPrefab != null)
                {
                    Vector3 position = main.transform.position;
                    var surfaceLayerMap = Instantiate(trackingPointMapPrefab, position, Quaternion.identity) as GameObject;
                    surfaceLayerMap.transform.parent = main.transform;

                    maps.Add(surfaceLayerMap);
                }
            }
        }

        StartCoroutine(ScrollBackgroundCoroutine());

        yield return null;
    }

    private TrackingPointMapPack.Map GetTrackingPointMap(TrackingPointMapPack surfacePack, int id)
    {
        TrackingPointMapPack.Pack pack = surfacePack.GetPack();

        foreach (TrackingPointMapPack.Map map in pack.maps)
        {
            if (map.id == id)
            {
                return map;
            }
        }

        return null;
    }

    private IEnumerator ScrollBackgroundCoroutine()
    {
        SetTrackingIdentifiers();

        Vector3 targetPosition = new Vector3(0.0f, transform.position.y - InGameManager.ScreenRatio.y, transform.position.z);
        float journeyLength = InGameManager.ScreenRatio.y;
        float accumulativeDeltaTime = 0.0f;
        bool complete = false;

        foreach (TrackingPointMapPack mapPack in trackingPointMapPacks)
        {
            TrackingPointMapPack.Map nextTrackingMap = GetTrackingPointMap(mapPack, bufferCanvasId);

            if (nextTrackingMap != null)
            {
                GameObject prefab = nextTrackingMap.prefab;

                if (prefab != null)
                {
                    Vector3 position = buffer.transform.position;
                    var surfaceLayerMap = Instantiate(prefab, position, Quaternion.identity) as GameObject;
                    surfaceLayerMap.transform.parent = buffer.transform;

                    actuators.Add((int) layer, CaptureActuators(surfaceLayerMap));
                }
            }
        }

        while (!complete)
        {
            float fractionComplete = accumulativeDeltaTime / journeyLength;

            transform.position = Vector3.Lerp(originPosition, targetPosition, (float)fractionComplete);

            StartCoroutine(ActionActuators());

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnComplete();
            }

            accumulativeDeltaTime += Time.deltaTime * scrollSpeed;

            yield return null;
        }
    }

    private void SetTrackingIdentifiers()
    {
        TextMesh textMesh;

        textMesh = mainIdTag.GetComponent<TextMesh>() as TextMesh;
        textMesh.color = idTagColor;
        textMesh.text = mainCanvasId.ToString();

        textMesh = bufferIdTag.GetComponent<TextMesh>() as TextMesh;
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

    private SortedList<float, GameObject> CaptureActuators(GameObject map)
    {
        var actuators = new SortedList<float, GameObject>(new DuplicateKeyComparer<float>());

        foreach (Transform childTransform in map.transform)
        {
            GameObject gameObject = childTransform.gameObject;

            var actuation = childTransform.gameObject.GetComponent<IActuate>() as IActuate;

            if ((actuation != null) && (childTransform.gameObject.activeSelf))
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
        }

        return actuators;
    }

    private IEnumerator ActionActuators()
    {
        foreach (KeyValuePair<long, SortedList<float, GameObject>> keyValuePair in actuators)
        {
            //long key = keyValuePair.Key;
            SortedList<float, GameObject> value = keyValuePair.Value;

            yield return StartCoroutine(ActionActuators(value));
        }
    }

    private IEnumerator ActionActuators(SortedList<float, GameObject> actuators)
    {
        while (actuators.Count > 0)
        {
            float key = actuators.Keys[0];
            GameObject gameObject = actuators.Values[0];

            float target = InGameManager.ScreenRatio.y - key;
            float delta = InGameManager.ScreenRatio.y + this.gameObject.transform.position.y - target;

            if (delta <= 0)
            {
                actuators.RemoveAt(0);

                var actuation = gameObject.GetComponent<IActuate>() as IActuate;

                if ((actuation != null) && (gameObject.activeSelf))
                {
                    actuation.Actuate();
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

        StartCoroutine(ScrollBackgroundCoroutine());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = idTagColor;
        DrawPanelGuide(transform.position);

        foreach (KeyValuePair<long, SortedList<float, GameObject>> keyValuePair in actuators)
        {
            //long key = keyValuePair.Key;
            SortedList<float, GameObject> value = keyValuePair.Value;

            if ((value != null) && (value.Count > 0))
            {
                foreach (KeyValuePair<float, GameObject> actuator in value)
                {
                    float key = actuator.Key;
                    GameObject gameObject = actuator.Value;

                    float target = InGameManager.ScreenRatio.y - key;
                    float delta = InGameManager.ScreenRatio.y + this.gameObject.transform.position.y - target;

                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(new Vector3(gameObject.transform.position.x - 0.5f, InGameManager.ScreenRatio.y + delta, gameObject.transform.position.z), new Vector3(gameObject.transform.position.x + 0.5f, InGameManager.ScreenRatio.y + delta, gameObject.transform.position.z));
                }
            }
        }
    }
}