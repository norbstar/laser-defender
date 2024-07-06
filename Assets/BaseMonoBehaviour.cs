using UnityEngine;

public abstract class BaseMonoBehaviour : SignatureMonoBehaviour
{
    public class Defaults
    {
        public Transform Transform { get; set; }
    }

    private Defaults defaults;

    public override void Awake()
    {
        base.Awake();

        defaults = new Defaults
        {
            Transform = transform
        };
    }

    public Transform GetTransform() => transform;

    public Defaults GetDefaults() => defaults;
}