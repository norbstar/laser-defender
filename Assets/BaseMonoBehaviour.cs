using System;

using UnityEngine;

public abstract class BaseMonoBehaviour : MonoBehaviour
{
    public class Defaults
    {
        public Transform Transform { get; set; }
    }

    private string guid;
    private Defaults defaults;

    public virtual void Awake()
    {
        defaults = new Defaults
        {
            Transform = transform
        };
    }

    public string Signature
    {
        get
        {
            guid = (guid == null) ? Guid.NewGuid().ToString() : guid;
            return guid;
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Defaults GetDefaults()
    {
        return defaults;
    }

    public void OverrideSignature(string guid)
    {
        this.guid = guid;
    }
}