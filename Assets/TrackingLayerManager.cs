using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TrackingLayerManager : MonoBehaviour
{
    public static Vector2 ScreenRatio = InGameManagerOld.ScreenRatio;

    [Header("Main")]
    [SerializeField] GameObject main;
    [SerializeField] TextMesh mainIdentifier;

    [Header("Buffer")]
    [SerializeField] GameObject buffer;
    [SerializeField] TextMesh bufferIdentifier;

    [Header("Gizmos")]
    [SerializeField] bool showGizmos;

    private int layer;
    private TrackingPointMapPack trackingPointMapPack;
    private Vector3 startPosition, lastPosition;
    private int primaryCanvasId, bufferCanvasId;
    private Color indicatorColor;
    private GameObject map;
    private SortedList<float, GameObject> actuators;
    private ActuatorManager actuatorManager;
    private float scrollSpeed;
    private int highestID, lowestID;

    void Awake()
    {
        primaryCanvasId = 0;
        bufferCanvasId = primaryCanvasId + 1;
        startPosition = transform.position;
        actuatorManager = FindObjectOfType<ActuatorManager>(); // ActuatorManager.Instance;
    }

    private int GetHighestID() => trackingPointMapPack.MapPack.maps.OrderByDescending(o => o.id).First().id;

    private int GetLowestID() => trackingPointMapPack.MapPack.maps.OrderByDescending(o => o.id).Last().id;

    public void Initiate(int layer, TrackingPointMapPack trackingPointMapPack, float scrollSpeed, Color indicatorColor)
    {
        this.layer = layer;
        this.trackingPointMapPack = trackingPointMapPack;
        this.indicatorColor = indicatorColor;
        this.scrollSpeed = scrollSpeed;

        if (trackingPointMapPack != null)
        {
            var primaryTrackingMap = GetTrackingPointMap(primaryCanvasId);

            if (primaryTrackingMap != null)
            {
                var prefab = primaryTrackingMap.prefab;

                if (prefab != null)
                {
                    var position = main.transform.position;
                    map = Instantiate(prefab, position, Quaternion.identity);
                    map.transform.parent = main.transform;
                }
            }
        }

        highestID = GetHighestID();
        lowestID = GetLowestID();
        StartCoroutine(Co_Scroll());
    }

    public void SetScrollSpeed(float scrollSpeed) => this.scrollSpeed = scrollSpeed;

    public Vector3 GetLastPosition() => lastPosition;

    private void UpdateIdentifiers()
    {
        mainIdentifier.color = indicatorColor;
        mainIdentifier.text = primaryCanvasId.ToString();
        bufferIdentifier.color = indicatorColor;
        bufferIdentifier.text = bufferCanvasId.ToString();
    }

    private IEnumerator Co_Scroll()
    {
        UpdateIdentifiers();

        var endPosition = new Vector3(0f, transform.position.y - ScreenRatio.y, transform.position.z);
        var speedAdjustedDeltaTime = 0f;
        var complete = false;

        if (trackingPointMapPack != null)
        {
            var nextTrackingMap = GetTrackingPointMap(bufferCanvasId);

            if (nextTrackingMap != null)
            {
                var prefab = nextTrackingMap.prefab;

                if (prefab != null)
                {
                    var position = buffer.transform.position;
                    map = Instantiate(prefab, position, Quaternion.identity);
                    map.transform.parent = buffer.transform;

                    actuators = CaptureActuators();
                }
            }
        }

        while (!complete)
        {
            var fractionComplete = speedAdjustedDeltaTime / ScreenRatio.y;

            lastPosition = transform.position;
            transform.position = Vector3.Lerp(startPosition, endPosition, fractionComplete);

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

    private TrackingPointMapPack.Map GetTrackingPointMap(int id)
    {
        var pack = trackingPointMapPack.MapPack;

        foreach (var map in pack.maps)
        {
            if (map.id == id)
            {
                return map;
            }
        }

        return null;
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
            var child = childTransform.gameObject;
            var actuation = child.GetComponent<IActuate>();
         
            child.layer = layer;

            if (actuation != null && childTransform.gameObject.activeSelf)
            {
                // Debug.Log($"CaptureActuators Child: {child.name} Layer: {layer}");

                var bounds = GeometryFunctions.GetAggregateBounds(child);

                if (bounds != null)
                {
                    // Use the bottom most bound of the game object
                    actuators.Add(bounds.Bottom, child);
                }
                else
                {
                    // Use the game objects position as there is no renderer to guage it's bottom most bounds
                    actuators.Add(childTransform.localPosition.y, child);
                }
            }
        }

        return actuators;
    }

    private IEnumerator Co_ActionActuators()
    {
        while (actuators.Count > 0)
        {
            var vHeight = actuators.Keys[0];
            var actuator = actuators.Values[0];
            var target = 16f - vHeight;
            var delta = 8f + gameObject.transform.position.y - target;

            if (delta <= 0)
            {
                actuators.RemoveAt(0);

                // Debug.Log($"Co_ActionActuators VHeight: {vHeight} Actuator: {actuator.name} Target: {target} Delta: {delta}");

                var actuation = actuator.GetComponent<IActuate>();

                if (actuation != null && actuator.activeSelf)
                {
                    actuation.Actuate();
                    // ActuatorManager.Add(gameObject);
                    actuatorManager.Add(actuator);
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
            // ActuatorManager.Remove(childTransform.gameObject);
            actuatorManager.Remove(childTransform.gameObject);
            Destroy(childTransform.gameObject);
        }

        for (int itr = 0; itr < buffer.transform.childCount; ++itr)
        {
            var childTransform = buffer.transform.GetChild(0);
            childTransform.position += new Vector3(0f, -ScreenRatio.y, 0f);
            childTransform.parent = main.transform;
        }

        // ++primaryCanvasId;

        // int nextId = primaryCanvasId + 1;
        int nextId = bufferCanvasId + 1;

        if (trackingPointMapPack.LoopOn && nextId > highestID)
        {
            // primaryCanvasId = 0;
            nextId = lowestID;
        }

        bufferCanvasId = nextId;
        transform.position = startPosition;

        StartCoroutine(Co_Scroll());
    }

    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = indicatorColor;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(9f, 0f, 0f));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(0f, ScreenRatio.y - transform.position.y, transform.position.z), new Vector3(1f, ScreenRatio.y - transform.position.y, transform.position.z));

        Gizmos.color = Color.blue;

        if (actuators != null)
        {
            foreach (KeyValuePair<float, GameObject> actuator in actuators)
            {
                var target = 16f - actuator.Key;
                var delta = 8f + transform.position.y - target;
                var gameObject = actuator.Value;
                
                Gizmos.DrawLine(new Vector3(0f, 16f + delta, gameObject.transform.position.z), new Vector3(1f, 16f + delta, gameObject.transform.position.z));
            }
        }
    }
}