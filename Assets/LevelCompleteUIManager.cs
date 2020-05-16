using System.Collections;

using UnityEngine;

public class LevelCompleteUIManager : MonoBehaviour, IActuate
{
    public delegate void OnLevelComplete();

    [SerializeField] TextUIManager textUIManager;

    private AudioSource audioSource;
    private OnLevelComplete onLevelCompleteDelegate;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void RegisterDelegate(OnLevelComplete onLevelCompleteDelegate)
    {
        this.onLevelCompleteDelegate = onLevelCompleteDelegate;
    }

    public void Actuate(IConfiguration configuration = null)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(ActuateCoroutine());
        }
    }

    private void ResolveDependencies() { }

    private IEnumerator ActuateCoroutine()
    {
        ResolveDependencies();

        textUIManager.transform.parent.gameObject.SetActive(true);
        onLevelCompleteDelegate?.Invoke();
        audioSource.Play();

        yield return new WaitForSeconds(1);

        textUIManager.transform.parent.gameObject.SetActive(false);
    }

    public void Deactivate() {
        textUIManager.gameObject.SetActive(false);
    }
}