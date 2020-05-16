using System.Collections;

using UnityEngine;

public class CountdownUIActuator : MonoBehaviour, IActuate
{
    private CountdownUIManager countdownUIManager;

    void Awake()
    {
        ResolveComponents();
    }

    public void Actuate(IConfiguration configuration = null)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(ActuateCoroutine());
        }
    }

    private void ResolveComponents()
    {
        countdownUIManager = FindObjectOfType<CountdownUIManager>() as CountdownUIManager;
    }

    private IEnumerator ActuateCoroutine()
    {
        countdownUIManager.RegisterDelegate(OnCountdownComplete);
        countdownUIManager.Actuate();

        yield return null;
    }

    public void OnCountdownComplete() { }
}