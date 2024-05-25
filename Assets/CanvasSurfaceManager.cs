using System.Collections;

using UnityEngine;

public class CanvasSurfaceManager : SurfaceManager, IActuate
{
    public class Configuration : GameplayConfiguration
    {
        public SpriteAssetPack SpriteAssetPack { get; set; }
    }

    private Sprite[] spritePack;
    private Vector3 originPosition;
    private SpriteRenderer mainCanvasRenderer, bufferCanvasRenderer;
    private int index, nextIndex;
    private int mainCanvasId, bufferCanvasId;
    private GameObject main, mainIdTag, buffer, bufferIdTag;

    public override void Awake()
    {
        base.Awake();

        ResolveComponents();

        index = 0;
        mainCanvasId = 0;
        bufferCanvasId = mainCanvasId + 1;
        originPosition = transform.position;
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration)
    {
        SpriteAssetPack spriteAssetPack = null;

        if (typeof(Configuration).IsInstanceOfType(configuration))
        {
            spriteAssetPack = ((Configuration) configuration).SpriteAssetPack;
        }

        spritePack = spriteAssetPack.Pack;

        index = 0;
        nextIndex = GetNextIndex();
        transform.position = originPosition;

        StartCoroutine(Co_Actuate());
    }

    private int GetNextIndex()
    {
        return (index + 1 <= spritePack.Length - 1) ? index + 1 : 0;
    }

    private void AssignCanvasSprites()
    {
        mainCanvasRenderer.sprite = spritePack[index];
        bufferCanvasRenderer.sprite = spritePack[nextIndex];
    }

    private void UpdateIdentifiers()
    {
        var textMesh = mainIdTag.GetComponent<TextMesh>();
        textMesh.text = mainCanvasId.ToString();

        textMesh = bufferIdTag.GetComponent<TextMesh>();
        textMesh.text = bufferCanvasId.ToString();
    }

    private void ResolveDependencies(GameObject prefab)
    {
        main = prefab.transform.Find("Main").gameObject as GameObject;
        mainCanvasRenderer = main.GetComponent<SpriteRenderer>();

        mainIdTag = prefab.transform.Find("Main Id Tag").gameObject as GameObject;

        buffer = prefab.transform.Find("Buffer").gameObject as GameObject;
        bufferCanvasRenderer = buffer.GetComponent<SpriteRenderer>();

        bufferIdTag = prefab.transform.Find("Buffer Id Tag").gameObject as GameObject;
    }

    private IEnumerator Co_Actuate()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GameObject prefab = Instantiate(surfaceLayerPrefab, transform.position, Quaternion.identity) as GameObject;
        prefab.transform.parent = transform;
        prefab.layer = (int) RenderLayer.CANVAS;
        prefab.name = "Canvas Surface Layer";

        //var surfaceLayerManager = prefab.GetComponent<SurfaceLayerManager>() as SurfaceLayerManager;
        //surfaceLayerManager.Actuate();

        ResolveDependencies(prefab);
        AssignCanvasSprites();
        UpdateIdentifiers();

        Vector3 targetPosition = new Vector3(0f, transform.position.y - InGameManager.ScreenRatio.y, transform.position.z);
        float distance = InGameManager.ScreenRatio.y;
        float speedAdjustedDeltaTime = 0f;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = speedAdjustedDeltaTime / distance;
            
            transform.position = Vector3.Lerp(originPosition, targetPosition, fractionComplete);

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnComplete();
            }

            speedAdjustedDeltaTime += scrollSpeed * Time.deltaTime;
            // Debug.Log($"Scroll Speed: {scrollSpeed} Speed Adjusted Delta Time: {speedAdjustedDeltaTime}");

            yield return null;
        }
    }

    private void OnComplete()
    {
        index = GetNextIndex();
        nextIndex = GetNextIndex();

        ++mainCanvasId;
        bufferCanvasId = mainCanvasId + 1;

        transform.position = originPosition;

        StartCoroutine(Co_Actuate());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = idTagColor;
        DrawPanelGuide(transform.position);
    }
}