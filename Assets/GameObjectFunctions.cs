using UnityEngine;

public class GameObjectFunctions
{
    // Resolve the sorting order id by layer
    public static int GetSortingOrderId(RenderLayer layer)
    {
        int id = 0;

        switch (layer)
        {
            case RenderLayer.CANVAS:
                id = SortingLayer.NameToID("Canvas Surface");
                break;

            case RenderLayer.BACKGROUND:
                id = SortingLayer.NameToID("Background Surface");
                break;

            case RenderLayer.SUB_SURFACE:
                id = SortingLayer.NameToID("Gameplay Subsurface");
                break;

            case RenderLayer.SURFACE:
                id = SortingLayer.NameToID("Gameplay Surface");
                break;

            case RenderLayer.FOREGROUND:
                id = SortingLayer.NameToID("Foreground Surface");
                break;
        }

        return id;
    }

    // Recursively designate the sorting layer based on the specified layer
    public static void DesignateSortingLayer(GameObject gameObject, int id)
    {
        var renderer = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;

        if (renderer != null)
        {
            renderer.sortingLayerID = id;
        }

        var canvas = gameObject.GetComponent<Canvas>() as Canvas;

        if (canvas != null)
        {
            canvas.sortingLayerID = id;
        }

        foreach (Transform childTransform in gameObject.transform)
        {
            DesignateSortingLayer(childTransform.gameObject, id);
        }
    }
}