using UnityEngine;

public class AbstractSceneryManager : MonoBehaviour
{
    public delegate void OnScrollSpeedUpdate(float scrollSpeed);

    [Header("Common")]
    [SerializeField] float scrollSpeed = 1.0f;

    private float? referenceScrollSpeed;
    private OnScrollSpeedUpdate onScrollSpeedUpdateDelegate;

    public virtual void Awake() => referenceScrollSpeed = scrollSpeed;

    public void RegisterDelegate(OnScrollSpeedUpdate onScrollSpeedUpdateDelegate) => this.onScrollSpeedUpdateDelegate = onScrollSpeedUpdateDelegate;

    public float GetReferenceScrollSpeed() => referenceScrollSpeed.Value;

    public float GetScrollSpeed() => scrollSpeed;

    public void SetScrollSpeed(float scrollSpeed)
    {
        this.scrollSpeed = scrollSpeed;
        onScrollSpeedUpdateDelegate?.Invoke(scrollSpeed);
    }
}