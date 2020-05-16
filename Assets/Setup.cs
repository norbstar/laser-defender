using UnityEngine;

public class Setup : MonoBehaviour, ISetup
{
    private RenderLayer activeLayer;

    void Awake()
    {
        activeLayer = RenderLayer.SURFACE;

        SetupHelper.SetSetupManager(this);
    }

    public RenderLayer GetActiveLayer()
    {
        return activeLayer;
    }
}