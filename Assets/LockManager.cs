using System;

using UnityEngine;

public class LockManager : MonoBehaviour
{
    [Serializable]
    public class Component
    {
        public GameObject gameObject;
        public bool lockPosition = false;
        [ReadOnly]
        public Vector3 position;
        public bool lockRotation = false;
        [ReadOnly]
        public Quaternion rotation;
    }

    [SerializeField] Component[] components;

    void Awake()
    {
        foreach (Component component in components)
        {
            component.position = component.gameObject.transform.position;
            component.rotation = component.gameObject.transform.rotation;
        }
    }

    void LateUpdate()
    {
        foreach (Component component in components)
        {
            if (component.lockPosition)
            {
                component.gameObject.transform.position = component.position;
            }

            if (component.lockRotation)
            {
                component.gameObject.transform.rotation = component.rotation;
            }
        }
    }
}