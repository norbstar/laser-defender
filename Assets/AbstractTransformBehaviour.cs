using System;

using UnityEngine;

public abstract class AbstractTransformBehaviour : MonoBehaviour
{
    [Serializable]
    public abstract class Feature
    {
        public bool enable = false;
    }

    [Serializable]
    public class Position : Feature
    {
        public Vector3 target;
    }

    [Serializable]
    public class Rotation : Feature
    {
        public Vector3 target;
    }

    [Serializable]
    public class Scale : Feature
    {
        public Vector3 target;
    }

    [SerializeField] protected Position position;
    [SerializeField] protected Rotation rotation;
    [SerializeField] protected Scale scale;

    protected Vector3 originPosition, originRotation, originScale;

    public virtual void Awake()
    {
        originPosition = transform.localPosition;
        originRotation = transform.localRotation.eulerAngles;
        originScale = transform.localScale;
    }
}