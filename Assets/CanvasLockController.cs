using UnityEngine;

public class CanvasLockController : MonoBehaviour
{
    [SerializeField] bool lockPosition = false;
    [SerializeField] bool lockRotation = false;

    private Vector3 position;
    private Quaternion rotation;

    void Awake()
    {
        position = transform.position;
        rotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (lockPosition)
        {
            transform.position = position;
        }

        if (lockRotation)
        {
            transform.rotation = rotation;
        }
    }
}