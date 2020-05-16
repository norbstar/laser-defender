using UnityEngine;

public class ConfirmExitGameActuator : MonoBehaviour, IActuate
{
    private GameObject subMenuItem;

    public void Actuate(IConfiguration configuration)
    {
        if (typeof(SubMenuItemConfiguration).IsAssignableFrom(configuration.GetType()))
        {
            this.subMenuItem = ((SubMenuItemConfiguration)configuration).SubMenuItem;
        }

        if (subMenuItem != null)
        {
            Debug.Log($"Exit Game: {subMenuItem.name}");
        }
    }
}