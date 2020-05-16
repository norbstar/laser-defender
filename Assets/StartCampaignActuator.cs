using UnityEngine;
using UnityEngine.SceneManagement;

public class StartCampaignActuator : MonoBehaviour, IActuate
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
            Debug.Log($"Start Campaign: {subMenuItem.name}");
        }

        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }
}