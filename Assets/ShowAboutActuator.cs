using UnityEngine;

public class ShowAboutActuator : MonoBehaviour, IActuate
{
    private GameObject subMenuItem;

    public void Actuate(IConfiguration configuration)
    {
        if (typeof(SubMenuItemConfiguration).IsInstanceOfType(configuration))
        {
            this.subMenuItem = ((SubMenuItemConfiguration) configuration).SubMenuItem;
        }

        if (subMenuItem != null)
        {
            Debug.Log($"Show About: {subMenuItem.name}");
        }
    }
}