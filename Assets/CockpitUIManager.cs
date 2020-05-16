using UnityEngine;

public class CockpitUIManager : MonoBehaviour
{
    [SerializeField] CockpitViewUIManager cockpitViewUIManager;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents() { }

    public CockpitViewUIManager GetCockpitViewUIManager()
    {
        return cockpitViewUIManager;
    }

    public void SetAsset(Sprite sprite)
    {
        cockpitViewUIManager.SetAsset(sprite);
    }

    public void ShowCockpit()
    {
        cockpitViewUIManager.ShowCockpit();
    }

    public void HideCockpit()
    {
        cockpitViewUIManager.HideCockpit();
    }
}