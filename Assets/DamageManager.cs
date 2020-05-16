using UnityEngine;

public class DamageManager
{
    public static void SetState(GameObject gameObject, bool enabled)
    {
        Collider2D collider = gameObject.GetComponent<Collider2D>() as Collider2D;

        if (collider != null)
        {
            collider.enabled = enabled;
        }

        var healthBarCanvas = gameObject.transform.Find("Health Bar Canvas");
        
        if (healthBarCanvas != null)
        {
            healthBarCanvas.gameObject.SetActive(enabled);
        }
    }
}