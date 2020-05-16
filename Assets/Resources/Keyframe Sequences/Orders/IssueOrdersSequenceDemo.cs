using UnityEngine;

public class IssueOrdersSequenceDemo : MonoBehaviour
{
    [SerializeField] IssueOrdersSequenceManager issueOrdersSequenceManager;

    // Start is called before the first frame update
    void Start()
    {
        issueOrdersSequenceManager.Actuate();
    }
}