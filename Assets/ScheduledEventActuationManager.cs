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
            StartCoroutine(ActuateCoroutine());
        }
    }

    private void ResolveDependencies() { }

    IEnumerator ActuateCoroutine()
    {
        ResolveDependencies();

        yield return null;
    }
}