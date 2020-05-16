using System;
using System.Collections.Generic;

using UnityEngine;

public class TestCases : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TestSortedList();
        //TestSortedDictionary();
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

    private void TestSortedList()
    {
        Debug.Log($"TestSortedList");

        var actuators = new SortedList<float, GameObject>(new DuplicateKeyComparer<float>());

        actuators.Add(0.1f, null);
        actuators.Add(0.25f, null);
        actuators.Add(0.1f, null);
        actuators.Add(0.5f, null);
        actuators.Add(1.0f, null);

        foreach (KeyValuePair<float, GameObject> actuator in actuators)
        {
            Debug.Log($"Key: {actuator.Key} Value: {actuator.Value}");
        }
    }

    private void TestSortedDictionary()
    {
        Debug.Log($"TestSortedDictionary");

        var actuators = new SortedDictionary<float, GameObject>();

        actuators.Add(0.1f, null);
        actuators.Add(0.25f, null);
        actuators.Add(0.1f, null);
        actuators.Add(0.5f, null);
        actuators.Add(1.0f, null);

        foreach (KeyValuePair<float, GameObject> actuator in actuators)
        {
            Debug.Log($"Key: {actuator.Key} Value: {actuator.Value}");
        }
    }   
}