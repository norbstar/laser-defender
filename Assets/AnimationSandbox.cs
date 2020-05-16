using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationSandbox : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject dynamicPlayer;

    [Range(0.5f, 2.0f)]
    public float exhaustScale = 1.0f;

    private PlayerController playerController;
    private DynamicPlayerController dynamicPlayerController;

    void Awake()
    {
        ResolveComponents();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController.EnableControls();
        dynamicPlayerController.EnableControls();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void ResolveComponents()
    {
        playerController = player.GetComponent<PlayerController>();
        dynamicPlayerController = dynamicPlayer.GetComponent<DynamicPlayerController>();
    }
}