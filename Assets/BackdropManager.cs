using System;
using System.Collections;

using UnityEngine;

public class BackdropManager : AbstractSceneryManager
{
    public delegate void OnDeactivated();

    [Header("Elements")]
    [SerializeField] GameObject main;
    [SerializeField] GameObject mainIdentifier;
    [SerializeField] GameObject buffer;
    [SerializeField] GameObject bufferIdentifier;

    public class AlreadyActiveException : Exception
    {
        public AlreadyActiveException() { }
        public AlreadyActiveException(string message) : base(message) { }
        public AlreadyActiveException(string message, Exception inner) : base(message, inner) { }
    }

    private OnDeactivated onDeactivatedDelegate;
    private Sprite[] spritePack;
    private Vector3 originPosition, lastPosition;
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
        originPosition = transform.position;
        isActive = shouldDeactivate = false;
    }

    public void RegisterDelegate(OnDeactivated onDeactivatedDelegate) => this.onDeactivatedDelegate = onDeactivatedDelegate;

    public bool IsActive() => isActive;

    public void Activate(SpriteAssetPack spriteAssetPack)
    {
        if (isActive)
        {
            throw new AlreadyActiveException();
        }

        spritePack = spriteAssetPack.Pack;

        index = 0;
        nextIndex = GetNextIndex();
        transform.position = originPosition;

        AssignCanvasSprites();
        StartCoroutine(ScrollBackgroundCoroutine());
    }

    public void Deactivate() => shouldDeactivate = true;

    public Vector2 GetLastPosition() => lastPosition;

    private void ResolveComponents()
    {
        primaryCanvasRenderer = main.GetComponent<SpriteRenderer>();
        nextCanvasRenderer = buffer.GetComponent<SpriteRenderer>();
    }

    private void SetTrackingIdentifiers()
    {
        var textMesh = mainIdentifier.GetComponent<TextMesh>() as TextMesh;
        textMesh.text = primaryCanvasId.ToString();

        textMesh = bufferIdentifier.GetComponent<TextMesh>() as TextMesh;
        textMesh.text = bufferCanvasId.ToString();
    }

    private IEnumerator ScrollBackgroundCoroutine()
    {
        SetTrackingIdentifiers();

        Vector3 targetPosition = new Vector3(0.0f, transform.position.y - InGameManagerOld.ScreenRatio.y, transform.position.z);
        //float magnitude = (targetPosition - originPosition).magnitude * 0.01f;
        //float startTransformTime = Time.time;
        float journeyLength = InGameManagerOld.ScreenRatio.y;
        float accumulativeDeltaTime = 0.0f;
        bool complete = false;
        isActive = true;

        while (!complete && !shouldDeactivate)
        {
            //float fractionComplete = (Time.time - startTransformTime) * GetSpeed(); /* (speed * magnitude);*/
            float fractionComplete = accumulativeDeltaTime / journeyLength;

            lastPosition = transform.position;
            transform.position = Vector3.Lerp(originPosition, targetPosition, (float) fractionComplete);

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnComplete();
            }

            accumulativeDeltaTime += Time.deltaTime * GetScrollSpeed();
            //Debug.Log($"Backdrop Manager -> Scroll Speed: {GetScrollSpeed()} Accumulative Delta Time: {accumulativeDeltaTime}");

            yield return null;
        }

        if (shouldDeactivate)
        {
            isActive = shouldDeactivate = false;
            onDeactivatedDelegate?.Invoke();
        }
    }

    private void AssignCanvasSprites()
    {
        primaryCanvasRenderer.sprite = spritePack[index];
        nextCanvasRenderer.sprite = spritePack[nextIndex];
    }

    private int GetNextIndex() => (index + 1 <= spritePack.Length - 1) ? index + 1 : 0;

    private void OnComplete()
    {
        index = GetNextIndex();
        nextIndex = GetNextIndex();

        ++primaryCanvasId;
        bufferCanvasId = primaryCanvasId + 1;

        transform.position = originPosition;

        AssignCanvasSprites();
        StartCoroutine(ScrollBackgroundCoroutine());
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