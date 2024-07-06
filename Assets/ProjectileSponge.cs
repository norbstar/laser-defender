using UnityEngine;

public class ProjectileSponge : MonoBehaviour
{
    private static string PROJECTILE_TAG = "Projectile";

    public void OnTriggerEnter2D(Collider2D collider)
    {
        var trigger = collider.gameObject;

        if (trigger.tag.Equals(PROJECTILE_TAG))
        {
            Destroy(trigger);
        }
    }
}
