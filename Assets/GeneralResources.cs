using UnityEngine;

public class GeneralResources : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject explosionPrefab;

    [Header("Folders")]
    [SerializeField] GameObject actuatorFolder;

    [Header("Canvas")]
    [SerializeField] GameObject scorePanel;
    [SerializeField] GameObject livesPanel;
    [SerializeField] GameObject countdownPanel;
    [SerializeField] GameObject expositionPanel;
    [SerializeField] GameObject levelCompletePanel;
    [SerializeField] GameObject cockpitPanel;

    public GameObject GetExplosionPrefab()
    {
        return explosionPrefab;
    }

    public GameObject GetActuatorFolder()
    {
        return actuatorFolder;
    }

    public GameObject GetScorePanel()
    {
        return scorePanel;
    }

    public GameObject GetLivesPanel()
    {
        return livesPanel;
    }

    public GameObject GetCountdownPanel()
    {
        return countdownPanel;
    }

    public GameObject GetExpositionPanel()
    {
        return expositionPanel;
    }

    public GameObject GetLevelCompletePanel()
    {
        return levelCompletePanel;
    }

    public GameObject GetCockpitPanel()
    {
        return cockpitPanel;
    }
}