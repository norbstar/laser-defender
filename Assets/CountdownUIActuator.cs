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
            StartCoroutine(Co_Actuate());
        }
    }

    private void ResolveComponents()
    {
        countdownUIManager = FindObjectOfType<CountdownUIManager>() as CountdownUIManager;
    }

    private IEnumerator Co_Actuate()
    {
        countdownUIManager.RegisterDelegate(OnCountdownComplete);
        countdownUIManager.Actuate();

        yield return null;
    }

    public void OnCountdownComplete() { }
}