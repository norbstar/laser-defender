using System.Collections;

using UnityEngine;

public abstract class AbstractActivationManager : MonoBehaviour
{
    public abstract IEnumerator Activate();
    public abstract void Deactivate();
}