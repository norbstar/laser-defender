using UnityEngine;

[CreateAssetMenu(menuName = "Issue Orders Sequence Configuration")]
public class IssueOrdersSequenceConfig : ScriptableObject
{
    [SerializeField] GameObject issueOrdersSequencePrefab;

    [Header("Cockpit")]
    [SerializeField] Sprite cockpitAsset;

    public GameObject GetIssueOrdersSequencePrefab()
    {
        return issueOrdersSequencePrefab;
    }

    public Sprite GetCockpitAsset()
    {
        return cockpitAsset;
    }
}