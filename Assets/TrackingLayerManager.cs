using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TrackingLayerManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] GameObject main, mainIdentifier;
    [SerializeField] GameObject buffer, bufferIdentifier;

    private TrackingPointMapPack trackingPointMapPack;
    private Vector3 originPosition, lastPosition;
    private int primaryCanvasId, bufferCanvasId;
    private Color indicatorColor;
    private GameObject map;
    private SortedList<float, GameObject> actuators;
    private float scrollSpeed;

    void Awake()
    {
        primaryCanvasId = 0;
        bufferCanvasId = primaryCanvasId + 1;
        originPosition = transform.position;
    }

    public void Initiate(TrackingPointMapPack trackingPointMapPack, float scrollSpeed, Color indicatorColor)
    {
        this.trackingPointMapPack = trackingPointMapPack;
        this.indicatorColor = indicatorColor;
        this.scrollSpeed = scrollSpeed;

        if (trackingPointMapPack != null)
        {
            TrackingPointMapPack.Map primaryTrackingMap = GetTrackingPointMap(primaryCanvasId);

            if (primaryTrackingMap != null)
            {
                GameObject prefab = primaryTrackingMap.prefab;

                if (prefab != null)
                {
                    Vector3 position = main.transform.position;
                    map = Instantiate(prefab, position, Quaternion.identity);
                    map.transform.parent = main.transform;
                }
            }
        }

        StartCoroutine(ScrollBackgroundCoroutine());
    }

    private void SetTrackingIdentifiers()
    {
        TextMesh textMesh;

        textMesh = mainIdentifier.GetComponent<TextMesh>();
        textMesh.color = indicatorColor;
        textMesh.text = primaryCanvasId.ToString();

        textMesh = bufferIdentifier.GetComponent<TextMesh>();
        textMesh.color = indicatorColor;
        textMesh.text = bufferCanvasId.ToString();
    }

    public void SetScrollSpeed(float scrollSpeed) => this.scrollSpeed = scrollSpeed;

    public Vector3 GetLastPosition() => lastPosition;

    private IEnumerator ScrollBackgroundCoroutine()
    {
        SetTrackingIdentifiers();

        Vector3 targetPosition = new Vector3(0.0f, transform.position.y - InGameManagerOld.ScreenRatio.y, transform.position.z);
        float journeyLength = InGameManagerOld.ScreenRatio.y;
        float accumulativeDeltaTime = 0.0f;
        bool complete = false;

        if (trackingPointMapPack != null)
        {
            TrackingPointMapPack.Map nextTrackingMap = GetTrackingPointMap(bufferCanvasId);

            if (nextTrackingMap != null)
            {
                GameObject prefab = nextTrackingMap.prefab;

                if (prefab != null)
                {
                    Vector3 position = buffer.transform.position;
                    map = Instantiate(prefab, position, Quaternion.identity);
                    map.transform.parent = buffer.transform;

                    actuators = CaptureActuators();
                }
            }
        }

        while (!complete)
        {
            var fractionComplete = accumulativeDeltaTime / journeyLength;

            lastPosition = transform.position;
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

            var actuation = childTransform.gameObject.GetComponent<IActuate>();

            if (actuation != null && childTransform.gameObject.activeSelf)
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
        if (actuators.Count > 0)
        {
            Debug.Log($"{name} ActionActuators Count: {actuators.Count}");
        }

        while (actuators.Count > 0)
        {
            var key = actuators.Keys[0];
            var gameObject = actuators.Values[0];
            var target = InGameManagerOld.ScreenRatio.y - key;
            var delta = InGameManagerOld.ScreenRatio.x + this.gameObject.transform.position.y - target;

            if (delta <= 0)
            {
                actuators.RemoveAt(0);

                var actuation = gameObject.GetComponent<IActuate>();

                if (actuation != null && gameObject.activeSelf)
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
            childTransform.position += new Vector3(0.0f, -InGameManagerOld.ScreenRatio.y, 0.0f);
            childTransform.parent = main.transform;
        }

        ++primaryCanvasId;
        bufferCanvasId = primaryCanvasId + 1;
        transform.position = originPosition;

        StartCoroutine(ScrollBackgroundCoroutine());
    }

#if false
    void OnDrawGizmos()
    {
        Gizmos.color = indicatorColor;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(9.0f, 0.0f, 0.0f));
        Gizmos.DrawLine(new Vector3(0.0f, InGameManager.ScreenRatio.y - transform.position.y, gameObject.transform.position.z), new Vector3(1.0f, InGameManager.ScreenRatio.y - transform.position.y, gameObject.transform.position.z));

        if (actuators != null)
        {
            foreach (KeyValuePair<float, GameObject> actuator in actuators)
            {
                float key = actuator.Key;
                GameObject gameObject = actuator.Value;

                float target = 16f - key;
                float delta = 8.0f + this.gameObject.transform.position.y - target;

                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(0.0f, 16.0f + delta, gameObject.transform.position.z), new Vector3(1.0f, 16.0f + delta, gameObject.transform.position.z));
            }
        }
    }
#endif
}