using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class OnTriggerHandler : MonoBehaviour
{
    public delegate void OnTrigger(GameObject gameObject);

    private OnTrigger onTriggerDelegate;

    public void RegisterDelegate(OnTrigger onTriggerDelegate)
    {
        this.onTriggerDelegate = onTriggerDelegate;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!InBoundsOfCamera(transform.position))
        {
            return;
        }

        if (collider != null)
        {
            GameObject trigger = collider.gameObject;

            onTriggerDelegate?.Invoke(trigger);
        }
    }

    private bool InBoundsOfCamera(Vector3 position)
    {
        return (position.x >= 0.0f) && (position.x <= InGameManager.ScreenWidthInUnits - 1.0f) &&
            (position.y >= 0.0f) && (position.y <= InGameManager.ScreenHeightInUnits - 1.0f);
    }
}