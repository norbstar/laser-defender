using System;
using System.Collections;

using UnityEngine;

public class BackdropManager : AbstractSceneryManager
{
    public static Vector2 ScreenRatio = InGameManagerOld.ScreenRatio;

    public delegate void OnDeactivated();

    [Header("Main")]
    [SerializeField] GameObject main;
    [SerializeField] TextMesh mainIdentifier;

    [Header("Buffer")]
    [SerializeField] GameObject buffer;
    [SerializeField] TextMesh bufferIdentifier;

    public class AlreadyActiveException : Exception
    {
        public AlreadyActiveException() { }
        public AlreadyActiveException(string message) : base(message) { }
        public AlreadyActiveException(string message, Exception inner) : base(message, inner) { }
    }

    private OnDeactivated onDeactivatedDelegate;
    private Sprite[] spritePack;
    private Vector3 startPosition, lastPosition;
    private SpriteRenderer primaryCanvasRenderer, nextCanvasRenderer;
    private int index, nextIndex;
    private bool isActive, shouldDeactivate;
    private int primaryCanvasId, bufferCanvasId;

    public override void Awake()
    {
        base.Awake();
        ResolveComponents();

        index = 0;
        primaryCanvasId = 0;
        bufferCanvasId = primaryCanvasId + 1;
        startPosition = transform.position;
        isActive = shouldDeactivate = false;
    }

    private void ResolveComponents()
    {
        primaryCanvasRenderer = main.GetComponent<SpriteRenderer>();
        nextCanvasRenderer = buffer.GetComponent<SpriteRenderer>();
    }

    public void RegisterDelegate(OnDeactivated onDeactivatedDelegate) => this.onDeactivatedDelegate = onDeactivatedDelegate;

    public bool IsActive() => isActive;

    public void Activate(SpriteAssetPack spriteAssetPack)
    {
        if (isActive) throw new AlreadyActiveException();

        spritePack = spriteAssetPack.Pack;

        index = 0;
        nextIndex = GetNextIndex();
        transform.position = startPosition;

        AssignCanvasSprites();
        StartCoroutine(Co_Scroll());
    }

    public void Deactivate() => shouldDeactivate = true;

    public Vector2 GetLastPosition() => lastPosition;

    private void UpdateIdentifiers()
    {
        mainIdentifier.text = primaryCanvasId.ToString();
        bufferIdentifier.text = bufferCanvasId.ToString();
    }

    private IEnumerator Co_Scroll()
    {
        UpdateIdentifiers();

        var endPosition = new Vector3(0f, transform.position.y - ScreenRatio.y, transform.position.z);
        var speedAdjustedDeltaTime = 0f;
        var complete = false;
        isActive = true;

        while (!complete && !shouldDeactivate)
        {
            var fractionComplete = speedAdjustedDeltaTime / ScreenRatio.y;

            lastPosition = transform.position;
            transform.position = Vector3.Lerp(startPosition, endPosition, fractionComplete);

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnComplete();
            }

            speedAdjustedDeltaTime += scrollSpeed * Time.deltaTime;
            // Debug.Log($"Scroll Speed: {scrollSpeed} Speed Adjusted Delta Time: {speedAdjustedDeltaTime}");

            yield return null;
        }

        if (shouldDeactivate)
        {
            isActive = shouldDeactivate = false;
            onDeactivatedDelegate?.Invoke();
        }
    }

    private int GetNextIndex() => (index + 1 <= spritePack.Length - 1) ? index + 1 : 0;

    private void AssignCanvasSprites()
    {
        primaryCanvasRenderer.sprite = spritePack[index];
        nextCanvasRenderer.sprite = spritePack[nextIndex];
    }

    private void OnComplete()
    {
        index = GetNextIndex();
        nextIndex = GetNextIndex();
        ++primaryCanvasId;
        bufferCanvasId = primaryCanvasId + 1;
        transform.position = startPosition;

        AssignCanvasSprites();
        StartCoroutine(Co_Scroll());
    }

#if (false)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(9.0f, 0.0f, 0.0f));

        Gizmos.DrawLine(new Vector3(gameObject.transform.position.x, InGameManager.ScreenHeightInUnits - transform.position.y, gameObject.transform.position.z), new Vector3(gameObject.transform.position.x + 0.5f, InGameManager.ScreenHeightInUnits - transform.position.y, gameObject.transform.position.z));
    }
#endif
}