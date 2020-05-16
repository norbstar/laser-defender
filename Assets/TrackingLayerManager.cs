using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TrackingLayerManager : MonoBehaviour
{
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
        ResolveComponents();

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
                    map = Instantiate(prefab, position, Quaternion.identity) as GameObject;
                    map.transform.parent = main.transform;
                }
            }
        }

        StartCoroutine(ScrollBackgroundCoroutine());
    }

    private void SetTrackingIdentifiers()
    {
        TextMesh textMesh;

        textMesh = mainIdentifier.GetComponent<TextMesh>() as TextMesh;
        textMesh.color = indicatorColor;
        textMesh.text = primaryCanvasId.ToString();

        textMesh = bufferIdentifier.GetComponent<TextMesh>() as TextMesh;
        textMesh.color = indicatorColor;
        textMesh.text = bufferCanvasId.ToString();
    }

    public void SetScrollSpeed(float scrollSpeed)
    {
        this.scrollSpeed = scrollSpeed;
    }

    public Vector3 GetLastPosition()
    {
        return lastPosition;
    }

    private void ResolveComponents() { }

    private IEnumerator ScrollBackgroundCoroutine()
    {
        SetTrackingIdentifiers();

        Vector3 targetPosition = new Vector3(0.0f, transform.position.y - InGameManagerOld.ScreenHeightInUnits, transform.position.z);
        //float magnitude = (targetPosition - originPosition).magnitude * 0.01f;
        //float startTransformTime = Time.time;
        float journeyLength = InGameManagerOld.ScreenHeightInUnits;
        //float lastJourneyLengthCovered = 0.0f;
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
                    map = Instantiate(prefab, position, Quaternion.identity) as GameObject;
                    map.transform.parent = buffer.transform;

                    actuators = CaptureActuators();
                }
            }
        }

        //float startTransformTime = Time.time;
        //float lastAccumulativeDeltaTime = 0.0f;
        //float lastJourneyLengthRemaining = journeyLength;

        while (!complete)
        {
            //float fractionComplete = (Time.time - startTransformTime) * speed; /*(speed * magnitude);*/
            //float fractionComplete = (Time.time - startTransformTime) / journeyLength;
            float fractionComplete = accumulativeDeltaTime / journeyLength;

            lastPosition = transform.position;
            transform.position = Vector3.Lerp(originPosition, targetPosition, (float) fractionComplete);

            StartCoroutine(ActionActuators());

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnComplete();
            }

            accumulativeDeltaTime += Time.deltaTime * scrollSpeed;
            //Debug.Log($"Tracking Layer Manager -> Scroll Speed: {scrollSpeed} Accumulative Delta Time: {accumulativeDeltaTime}");

            //float journeyLengthRemaining = journeyLength - (fractionComplete * journeyLength);
            //float sampleTime = lastAccumulativeDeltaTime - accumulativeDeltaTime;
            //Debug.Log($"Est Speed: {journeyLengthRemaining / sampleTime}");

            //float relativeTimeElapsed = accumulativeDeltaTime - Time.deltaTime;
            //Debug.Log($"Relative Time Elapsed: {relativeTimeElapsed}");

            //float relativeJourneyCovered = fractionComplete * journeyLength;
            //Debug.Log($"Relative Journey Covered: {relativeJourneyCovered}");

            //Debug.Log($"Est Speed: {(fractionComplete * journeyLength) / accumulativeDeltaTime}");

            //float journeyRemaining = journeyLength - (fractionComplete * journeyLength);
            //Debug.Log($"Time Elapsed: {accumulativeDeltaTime} Journey Remaining: {journeyRemaining}");
            //Debug.Log($"Speed: {journeyRemaining / accumulativeDeltaTime}");

            //lastJourneyLengthRemaining = journeyLengthRemaining;
            //lastAccumulativeDeltaTime += accumulativeDeltaTime;

            //float journeyLengthCovered = fractionComplete * journeyLength;
            //float journeyDelta = journeyLengthCovered - lastJourneyLengthCovered;
            //float speed = journeyDelta / Time.deltaTime;
            //Debug.Log($"Journey Length Covered: {journeyLengthCovered} Journey Delta: {journeyDelta} Delta Time: {Time.deltaTime} Speed: {speed}");
            //Debug.Log($"Delta Time: {Time.deltaTime} Journey Delta: {journeyDelta} Speed: {speed}");

            //lastJourneyLengthCovered = fractionComplete * journeyLength;

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
        while (actuators.Count > 0)
        {
            float key = actuators.Keys[0];
            GameObject gameObject = actuators.Values[0];

            float target = 16.0f - key;
            float delta = ((8.0f + this.gameObject.transform.position.y) - target);

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
            childTransform.position += new Vector3(0.0f, -InGameManagerOld.ScreenHeightInUnits, 0.0f);
            childTransform.parent = main.transform;
        }

        ++primaryCanvasId;
        bufferCanvasId = primaryCanvasId + 1;
        transform.position = originPosition;

        StartCoroutine(ScrollBackgroundCoroutine());
    }

#if (false)
    void OnDrawGizmos()
    {
        Gizmos.color = indicatorColor;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(9.0f, 0.0f, 0.0f));
        Gizmos.DrawLine(new Vector3(0.0f, InGameManager.ScreenHeightInUnits - transform.position.y, gameObject.transform.position.z), new Vector3(1.0f, InGameManager.ScreenHeightInUnits - transform.position.y, gameObject.transform.position.z));

        if (actuators != null)
        {
            foreach (KeyValuePair<float, GameObject> actuator in actuators)
            {
                float key = actuator.Key;
                GameObject gameObject = actuator.Value;

                float target = 16.0f - key;
                float delta = ((8.0f + this.gameObject.transform.position.y) - target);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(0.0f, 16.0f + delta, gameObject.transform.position.z), new Vector3(1.0f, 16.0f + delta, gameObject.transform.position.z));
            }
        }
    }
#endif
}