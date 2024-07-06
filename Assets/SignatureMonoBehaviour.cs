using System;

using UnityEngine;

public abstract class SignatureMonoBehaviour : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] string signature;

    private string guid;

    public virtual void Awake() => signature = Guid.NewGuid().ToString();

    public string Signature { get { return guid = (guid == null) ? signature : guid; } set => signature = value; }

    public bool SharesSameSignature(GameObject obj)
    {
        if (obj.TryGetComponent<SignatureMonoBehaviour>(out var signatureMonoBehaviour))
        {
            return signatureMonoBehaviour.Signature.Equals(Signature);
        }

        return false;
    }
}