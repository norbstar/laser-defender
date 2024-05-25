using System.Collections;

public class IssueOrdersCutSceneActuator : CutSceneActuator, IActuate
{
    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(Co_Actuate());
        }
    }

    private IEnumerator Co_Actuate()
    {
        ResolveDependencies();

        yield return null;
    }

    private void ResolveDependencies() { }
}