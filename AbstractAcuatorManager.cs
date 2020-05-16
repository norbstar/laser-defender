using UnityEngine;

public abstract class AbstractActuatorManager : MonoBehaviour
{
    public abstract void Actuate(AbstractConfig config);
}