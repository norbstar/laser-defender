using UnityEngine;

public class ShotSpongeDetector : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject trigger = collider.gameObject;

        if (trigger.tag.Equals("Projectile"))
        {
            Destroy(trigger);
        }
    }
}
