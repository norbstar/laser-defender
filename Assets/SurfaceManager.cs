using UnityEngine;

public abstract class SurfaceManager : MonoBehaviour
{
    public delegate void OnScrollSpeedUpdate(float scrollSpeed);

    [SerializeField] SurfacesManager surfacesManager;

    [Header("Surface")]
    [SerializeField] protected GameObject surfaceLayerPrefab;
    [SerializeField] [ShowOnly] protected float scrollSpeed = 1.0f;
    [SerializeField] protected Color idTagColor;

    private float defaultScrollSpeed;
    private float panelGuideOffset;
    private OnScrollSpeedUpdate onScrollSpeedUpdateDelegate;

    public virtual void Awake()
    {
        scrollSpeed = defaultScrollSpeed = 1.0f / transform.position.z;
    }

    public void RegisterDelegate(OnScrollSpeedUpdate onScrollSpeedUpdateDelegate)
    {
        this.onScrollSpeedUpdateDelegate = onScrollSpeedUpdateDelegate;
    }

    public float GetDefaultScrollSpeed()
    {
        return defaultScrollSpeed;
    }

    public float GetScrollSpeed()
    {
        return scrollSpeed;
    }

    public void SetScrollSpeed(float scrollSpeed)
    {
        this.scrollSpeed = scrollSpeed;

        onScrollSpeedUpdateDelegate?.Invoke(scrollSpeed);
    }

    public void SetPanelGuideOffset(float offset)
    {
        panelGuideOffset = offset;
    }

    protected void DrawPanelGuide(Vector3 position)
    {
        Gizmos.DrawLine(position + new Vector3(-panelGuideOffset - 0.5f, InGameManager.ScreenRatio.y, 0.0f), position + new Vector3(-panelGuideOffset + 0.5f, InGameManager.ScreenRatio.y, 0.0f));
        Gizmos.DrawCube(position + new Vector3(-panelGuideOffset, InGameManager.ScreenRatio.y / 2f, 0.0f), new Vector3(0.5f, InGameManager.ScreenRatio.y, 0.5f));
        Gizmos.DrawCube(position + new Vector3(-panelGuideOffset, InGameManager.ScreenRatio.y * 1.5f, 0.0f), new Vector3(0.5f, InGameManager.ScreenRatio.y, 0.5f));
    }
}