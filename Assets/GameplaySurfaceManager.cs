using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameplaySurfaceManager : SurfaceManager, IActuate
{
    public class Dependencies
    {
        public GameObject Main { get; set; }
        public GameObject MainIdTag { get; set; }
        public GameObject Buffer { get; set; }
        public GameObject BufferIdTag { get; set; }
    }

    public class Configuration : GameplayConfiguration
    {
        public GameplaySurfacePack SurfacePack { get; set; }
    }

    private TrackingPointMapPack subSurfaceTrackingPointMapPack, surfaceTrackingPointMapPack;
    private Vector3 originPosition;
    private int index;
    private SortedList<float, GameObject> subSurfaceActuators, surfaceActuators;
    private int mainCanvasId, bufferCanvasId;
    private GameObject subSurfaceMap, surfaceMap;
    private GameObject gameplaySubsurfacePrefab, gameplaySurfacePrefab;

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
        GameplaySurfacePack surfacePack = null;

        if (typeof(Configuration).IsInstanceOfType(configuration))
        {
            surfacePack = ((Configuration) configuration).SurfacePack;
        }

        GameplaySurfacePack.Pack pack = surfacePack.GetPack();

        if (pack.enableTracking)
        {
            subSurfaceTrackingPointMapPack = pack.subSurfaceTrackingPointMapPack;
            surfaceTrackingPointMapPack = pack.surfaceTrackingPointMapPack;
        }

        StartCoroutine(ActuateCoroutine());
    }

    private Dependencies ResolveDependencies(GameObject prefab)
    {
        if (prefab != null)
        {
            return new Dependencies
            {
                Main = prefab.transform.Find("Main").gameObject as GameObject,
                MainIdTag = prefab.transform.Find("Main Id Tag").gameObject as GameObject,
                Buffer = prefab.transform.Find("Buffer").gameObject as GameObject,
                BufferIdTag = prefab.transform.Find("Buffer Id Tag").gameObject as GameObject
            };
        }

        return null;
    }

    private IEnumerator ActuateCoroutine()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (subSurfaceTrackingPointMapPack != null)
        {
            gameplaySubsurfacePrefab = Instantiate(surfaceLayerPrefab, transform.position, Quaternion.identity) as GameObject;
            gameplaySubsurfacePrefab.transform.parent = transform;
            gameplaySubsurfacePrefab.layer = (int) RenderLayer.SUB_SURFACE;
            gameplaySubsurfacePrefab.name = "Gameplay Subsurface Layer";

            //var surfaceLayerManager = gameplaySubsurfacePrefab.GetComponent<SurfaceLayerManager>() as SurfaceLayerManager;
            //surfaceLayerManager.Actuate(new SurfaceLayerManager.Configuration
            //{
            //    ZDepth = 0.1f
            //});

            TrackingPointMapPack.Map subSurfaceTrackingPointMap = GetTrackingPointMap(subSurfaceTrackingPointMapPack, mainCanvasId);

            if (subSurfaceTrackingPointMap != null)
            {
                GameObject trackingPointMapPrefab = subSurfaceTrackingPointMap.prefab;

                if (trackingPointMapPrefab != null)
                {
                    Dependencies dependencies = ResolveDependencies(gameplaySubsurfacePrefab);

                    if (dependencies != null)
                    {
                        Vector3 position = dependencies.Main.transform.position;
                        subSurfaceMap = Instantiate(trackingPointMapPrefab, position, Quaternion.identity) as GameObject;
                        subSurfaceMap.transform.parent = dependencies.Main.transform;
                    }
                }
            }
        }

        if (surfaceTrackingPointMapPack != null)
        {
            gameplaySurfacePrefab = Instantiate(surfaceLayerPrefab, transform.position, Quaternion.identity) as GameObject;
            gameplaySurfacePrefab.transform.parent = transform;
            gameplaySurfacePrefab.layer = (int) RenderLayer.SURFACE;
            gameplaySurfacePrefab.name = "Gameplay Surface Layer";

            //var surfaceLayerManager = gameplaySubsurfacePrefab.GetComponent<SurfaceLayerManager>() as SurfaceLayerManager;
            //surfaceLayerManager.Actuate();

            TrackingPointMapPack.Map surfaceTrackingPointMap = GetTrackingPointMap(surfaceTrackingPointMapPack, mainCanvasId);

            if (surfaceTrackingPointMap != null)
            {
                GameObject trackingPointMapPrefab = surfaceTrackingPointMap.prefab;

                if (trackingPointMapPrefab != null)
                {
                    Dependencies dependencies = ResolveDependencies(gameplaySurfacePrefab);

                    if (dependencies != null)
                    {
                        Vector3 position = dependencies.Main.transform.position;
                        surfaceMap = Instantiate(trackingPointMapPrefab, position, Quaternion.identity) as GameObject;
                        surfaceMap.transform.parent = dependencies.Main.transform;
                    }
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
        if (subSurfaceTrackingPointMapPack != null)
        {
            Dependencies dependencies = ResolveDependencies(gameplaySubsurfacePrefab);
            SetTrackingIdentifiers(dependencies);
            TrackingPointMapPack.Map subSurfaceTrackingPointMap = GetTrackingPointMap(subSurfaceTrackingPointMapPack, bufferCanvasId);

            if (subSurfaceTrackingPointMap != null)
            {
                GameObject trackingPointMapPrefab = subSurfaceTrackingPointMap.prefab;

                if (trackingPointMapPrefab != null)
                {
                    Vector3 position = dependencies.Buffer.transform.position;
                    subSurfaceMap = Instantiate(trackingPointMapPrefab, position, Quaternion.identity) as GameObject;
                    subSurfaceMap.transform.parent = dependencies.Buffer.transform;

                    subSurfaceActuators = CaptureActuators(subSurfaceMap);
                }
            }
        }

        if (surfaceTrackingPointMapPack != null)
        {
            Dependencies dependencies = ResolveDependencies(gameplaySurfacePrefab);
            SetTrackingIdentifiers(dependencies);
            TrackingPointMapPack.Map nextTrackingMap = GetTrackingPointMap(surfaceTrackingPointMapPack, bufferCanvasId);

            if (nextTrackingMap != null)
            {
                GameObject trackingPointMapPrefab = nextTrackingMap.prefab;

                if (trackingPointMapPrefab != null)
                {
                    Vector3 position = dependencies.Buffer.transform.position;
                    surfaceMap = Instantiate(trackingPointMapPrefab, position, Quaternion.identity) as GameObject;
                    surfaceMap.transform.parent = dependencies.Buffer.transform;

                    surfaceActuators = CaptureActuators(surfaceMap);
                }
            }
        }

        Vector3 targetPosition = new Vector3(0.0f, transform.position.y - InGameManager.ScreenRatio.y, transform.position.z);
        float journeyLength = InGameManager.ScreenRatio.y;
        float accumulativeDeltaTime = 0.0f;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = accumulativeDeltaTime / journeyLength;

            transform.position = Vector3.Lerp(originPosition, targetPosition, (float) fractionComplete);

            StartCoroutine(ActionActuators());

            complete = fractionComplete >= 1.0f;

            if (complete)
            {
                OnComplete();
            }

            accumulativeDeltaTime += Time.deltaTime * scrollSpeed;

            yield return null;
        }
    }

    private void AssignLayerToGroup(Transform transform, RenderLayer layer)
    {
        foreach (Transform childTransform in transform)
        {
            childTransform.gameObject.layer = (int) layer;
            AssignLayerToGroup(childTransform, layer);
        }
    }

    private void SetTrackingIdentifiers(Dependencies dependencies)
    {
        TextMesh textMesh;

        textMesh = dependencies.MainIdTag.GetComponent<TextMesh>() as TextMesh;
        textMesh.color = idTagColor;
        textMesh.text = mainCanvasId.ToString();

        textMesh = dependencies.BufferIdTag.GetComponent<TextMesh>() as TextMesh;
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
            else
            {
                // Add a layer actuator to the game object
                childTransform.gameObject.AddComponent<LayerActuator>();
                actuators.Add(0.0f, childTransform.gameObject);
            }
        }

        return actuators;
    }

    private IEnumerator ActionActuators()
    {
        yield return StartCoroutine(ActionActuators(RenderLayer.SUB_SURFACE, subSurfaceActuators));
        yield return StartCoroutine(ActionActuators(RenderLayer.SURFACE, surfaceActuators));
    }

    private IEnumerator ActionActuators(RenderLayer layer, SortedList<float, GameObject> actuators)
    {
        if (actuators != null)
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

                    if (actuation != null && gameObject.activeSelf)
                    {
                        actuation.Actuate(new GameplayConfiguration
                        {
                            Layer = layer
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
    }

    public float GetMultiplier(GameObject gameObject, int layer)
    {
        float multiplier = 1.0f;

        int childLayer = gameObject.layer;

        if (layer == (int) RenderLayer.SURFACE)
        {
            if (childLayer == (int) RenderLayer.SUB_SURFACE)
            {
                return 0.9f;
            }
        }
        else if (layer == (int) RenderLayer.SUB_SURFACE)
        {
            if (childLayer == (int) RenderLayer.SURFACE)
            {
                return 1.1f;
            }
        }

        return multiplier;
    }

    private void OnComplete()
    {
        Dependencies dependencies;

        dependencies = ResolveDependencies(gameplaySubsurfacePrefab);

        if (dependencies != null)
        {
            OnComplete(dependencies);
        }

        dependencies = ResolveDependencies(gameplaySurfacePrefab);

        if (dependencies != null)
        {
            OnComplete(dependencies);
        }

        ++index;
        bufferCanvasId = index + 1;
        transform.position = originPosition;

        StartCoroutine(ScrollBackgroundCoroutine());
    }

    private void OnComplete(Dependencies dependencies)
    {
        foreach (Transform childTransform in dependencies.Main.transform)
        {
            Destroy(childTransform.gameObject);
        }

        for (int itr = 0; itr < dependencies.Buffer.transform.childCount; ++itr)
        {
            Transform childTransform = dependencies.Buffer.transform.GetChild(0);
            childTransform.position += new Vector3(0.0f, -InGameManager.ScreenRatio.y, 0.0f);
            childTransform.parent = dependencies.Main.transform;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = idTagColor;
        DrawPanelGuide(transform.position);

        if ((surfaceActuators != null) && (surfaceActuators.Count > 0))
        {
            foreach (KeyValuePair<float, GameObject> actuator in surfaceActuators)
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

        if ((subSurfaceActuators != null) && (subSurfaceActuators.Count > 0))
        {
            foreach (KeyValuePair<float, GameObject> actuator in subSurfaceActuators)
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