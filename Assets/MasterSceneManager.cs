using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterSceneManager : MonoBehaviour
{
    public delegate void OnSceneLoadComplete(string sceneName);

    private string sceneName;
    private string activeSceneName;
    private AsyncOperation sceneLoaderAsync;
    private OnSceneLoadComplete onSceneLoadCompleteDelegate;

    // Update is called once per frame
    void Update()
    {
        if ((sceneLoaderAsync != null) && (sceneLoaderAsync.isDone))
        {
            sceneLoaderAsync = null;
            activeSceneName = sceneName;
            onSceneLoadCompleteDelegate?.Invoke(sceneName);
        }
    }

    public void SetScene(string sceneName)
    {
        if (activeSceneName != null)
        {
            UnloadScene(activeSceneName);
        }

        if (sceneName != null)
        {
            LoadScene(sceneName);
        }
    }

    public void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single)
    {
        string sceneName = BuildIndexToSceneName(sceneIndex);

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            this.sceneName = sceneName;
            sceneLoaderAsync = SceneManager.LoadSceneAsync(sceneName, mode);
        }
    }

    private string BuildIndexToSceneName(int sceneIndex)
    {
        return System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(sceneIndex));
    }

    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            this.sceneName = sceneName;
            sceneLoaderAsync = SceneManager.LoadSceneAsync(sceneName, mode);
        }
    }

    public void UnloadScene(string sceneName)
    {
        if (sceneName != null)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }

    public void RegisterDelegate(OnSceneLoadComplete onSceneLoadCompleteDelegate)
    {
        this.onSceneLoadCompleteDelegate = onSceneLoadCompleteDelegate;
    }
}