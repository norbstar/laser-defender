using System.Collections;

using UnityEngine;

public class ScheduledEventActuationManager : MonoBehaviour, IActuate
{
    [SerializeField] ScheduleEventConfig scheduleEventConfig;

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

    private void ResolveDependencies() { }

    private IEnumerator Co_Actuate()
    {
        ResolveDependencies();

        yield return null;
    }
}