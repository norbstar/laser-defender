using System;
using System.Collections.Generic;

using UnityEngine;

public class PathConfig : MonoBehaviour
{
    [Serializable]
    public class Smoothing
    {
        public bool xTangents = false;
        public bool yTangents = false;
    }

    [SerializeField] Smoothing smoothing;

    public Smoothing GetSmoothing()
    {
        return smoothing;
    }

    public IList<GameObject> GetWaypoints()
    {
        var waypoints = new List<GameObject>();

        foreach (Transform childTransform in transform)
        {
            waypoints.Add(childTransform.gameObject);
        }

        return waypoints;
    }
}