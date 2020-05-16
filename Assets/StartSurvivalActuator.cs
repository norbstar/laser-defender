using UnityEngine;

public class StartSurvivalActuator : MonoBehaviour, IActuate
{
    private GameObject subMenuItem;

    public void Actuate(IConfiguration configuration)
    {
        if (typeof(SubMenuItemConfiguration).IsInstanceOfType(configuration))
        {
            subMenuItem = ((SubMenuItemConfiguration)configuration).SubMenuItem;
        }

        if (subMenuItem != null)
        {
            Debug.Log($"Start Survival: {subMenuItem.name}");
        }
    }
}