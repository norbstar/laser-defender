using System.Collections;

using UnityEngine;

public class HealthBarManagerDemo : MonoBehaviour
{
    [SerializeField] HealthBarSliderUIManager healthBarSliderUIManager;

    private float minHealth, maxHealth;

    void Awake()
    {
        maxHealth = healthBarSliderUIManager.GetMaxHealth();
        minHealth = healthBarSliderUIManager.GetMinHealth();
    }

    IEnumerator Start()
    {
        while (true)
        {
            float health = healthBarSliderUIManager.GetHealth();
            healthBarSliderUIManager.SetHealth((health - 1 >= minHealth) ? health - 1 : maxHealth);

            yield return new WaitForSeconds(0.1f);
        }
    }
}