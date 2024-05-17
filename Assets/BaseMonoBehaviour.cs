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

        signature = Guid.NewGuid().ToString();
    }

    public string Signature
    {
        get
        {
            //guid = (guid == null) ? Guid.NewGuid().ToString() : guid;
            guid = (guid == null) ? signature : guid;
            return guid;
        }
    }

    public Transform GetTransform() => transform;

    public Defaults GetDefaults() => defaults;

    public void OverrideSignature(string guid) => this.guid = guid;

    [SerializeField] string signature;
}