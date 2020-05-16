using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != null)
        {
            //if (collider.gameObject.layer != gameObject.layer)
            //{
            //    return;
            //}
            
            GameObject origin = collider.gameObject;
            Debug.Log($"Source: {gameObject.name} Collider: {origin.name}");
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            //if (collision.gameObject.layer != gameObject.layer)
            //{
            //    return;
            //}
            
            GameObject origin = collision.gameObject;
            Debug.Log($"Source: {gameObject.name} Collision: {origin.name}");
        }
    }
}