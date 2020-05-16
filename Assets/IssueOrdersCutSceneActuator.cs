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
            StartCoroutine(ActuateCoroutine());
        }
    }

    private IEnumerator ActuateCoroutine()
    {
        ResolveDependencies();

        yield return null;
    }

    private void ResolveDependencies() { }
}