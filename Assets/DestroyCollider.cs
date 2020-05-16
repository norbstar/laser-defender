using System.Linq;

using UnityEngine;

public class DestroyCollider : MonoBehaviour
{
    [SerializeField] string[] tags;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject trigger = collider.gameObject;

        if (tags.Contains(trigger.tag))
        {
            Destroy(trigger);
        }
    }
}