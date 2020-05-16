using UnityEngine;
//using UnityEngine.SceneManagement;

public class InGameManagerOriginal : MonoBehaviour
{
    public static float ScreenWidthInUnits = 9.0f;
    public static float ScreenHeightInUnits = 16.0f;

    [SerializeField] LevelOld[] levels;
    [SerializeField] CountdownUIManager countdownUIManager;
    
    //private MasterSceneManager masterSceneManager;
    private LivesUIManager livesUIManager;
    //private string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        ResolveComponents();

        Cursor.visible = false;
        
        //countdownUIManager.StartCoroutine(countdownUIManager.InitiateCountdown());
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Escape))
    //    {
    //        masterSceneManager.LoadScene(0);
    //    }
    //}

    private void ResolveComponents()
    {
        //masterSceneManager = FindObjectOfType<MasterSceneManager>();
        //masterSceneManager.RegisterDelegate(SceneLoadComplete);

        livesUIManager = FindObjectOfType<LivesUIManager>();
    }

    //public void SceneLoadComplete(string sceneName)
    //{
    //    this.sceneName = sceneName;

    //    countdownUIManager.StartCoroutine(countdownUIManager.InitiateCountdown());
    //}

    private void RevokeLife()
    {
        livesUIManager.RemoveLife();
    }

    private bool HasLives()
    {
        return livesUIManager.HasLives();
    }
}