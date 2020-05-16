using System.Collections.Generic;

using UnityEngine;

public class TrackingPointMapManager : MonoBehaviour
{
    public IList<GameObject> GetTrackingPoints()
    {
        IList<GameObject> trackingPoints = new List<GameObject>();

        foreach (Transform childTransform in transform)
        {
            trackingPoints.Add(childTransform.gameObject);
        }

        return trackingPoints;
    }
}