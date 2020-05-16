using UnityEngine;

public class ActuateGameObjects : MonoBehaviour
{
    [SerializeField] GameObject player;

    private DynamicPlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<DynamicPlayerController>() as DynamicPlayerController;
        EnablePlayerControls();
    }

    private void EnablePlayerControls() => playerController.EnableControls();
}