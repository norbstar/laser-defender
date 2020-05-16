using UnityEngine;

public class SurfaceManager : MonoBehaviour
{
    public delegate void OnScrollSpeedUpdate(float scrollSpeed);

    [SerializeField] SurfacesManager surfacesManager;

    [Header("Surface")]
    [SerializeField] protected GameObject surfaceLayerPrefab;
    [SerializeField] [ShowOnly] protected float scrollSpeed = 1.0f;
    [SerializeField] protected Color idTagColor;

    private float defaultScrollSpeed;
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

    protected void DrawPanelGuide(Vector3 position, float xOffset)
    {
        Gizmos.DrawLine(position + new Vector3(-xOffset - 0.5f, InGameManager.ScreenHeightInUnits, 0.0f), position + new Vector3(-xOffset + 0.5f, InGameManager.ScreenHeightInUnits, 0.0f));
        Gizmos.DrawCube(position + new Vector3(-xOffset, InGameManager.ScreenHeightInUnits / 2f, 0.0f), new Vector3(0.5f, InGameManager.ScreenHeightInUnits, 0.5f));
        Gizmos.DrawCube(position + new Vector3(-xOffset, InGameManager.ScreenHeightInUnits * 1.5f, 0.0f), new Vector3(0.5f, InGameManager.ScreenHeightInUnits, 0.5f));
    }
}