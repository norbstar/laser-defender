using UnityEngine;

public class GeometryFunctions : MonoBehaviour
{
    public class Bounds
    {
        public float Top { get; set; }
        public float Left { get; set; }
        public float Bottom { get; set; }
        public float Right { get; set; }
    }

    // Resolve the absolute positional bounds of the specified game object
    public static Bounds GetBounds(GameObject gameObject)
    {
        var renderer = gameObject.GetComponent<Renderer>() as Renderer;

        if (renderer != null)
        {
            Vector3 position = gameObject.transform.position;
            position -= new Vector3(0.0f, InGameManager.ScreenRatio.y, 0.0f);
            Vector3 size = renderer.bounds.size;

            float top = position.y + (size.y / 2);
            float left = position.x - (size.x / 2);
            float bottom = position.y - (size.y / 2);
            float right = position.x + (size.x / 2);

            return new Bounds
            {
                Top = top,
                Left = left,
                Bottom = bottom,
                Right = right
            };
        }

        return null;
    }

    // Resolve the absolute aggregate positional bounds of the specified game object
    public static Bounds GetAggregateBounds(GameObject gameObject)
    {
        Bounds aggregateBounds = null;
        Bounds bounds = GetBounds(gameObject);

        if (bounds != null)
        {
            aggregateBounds = bounds;
        }

        foreach (Transform childTransform in gameObject.transform)
        {
            bounds = GetAggregateBounds(childTransform.gameObject);

            if (bounds != null)
            {
                Vector2 position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - InGameManager.ScreenRatio.y);
                
                aggregateBounds = new Bounds
                {
                    Top = position.y,
                    Left = position.x,
                    Bottom = position.y,
                    Right = position.x
                };

                if (bounds.Top > aggregateBounds.Top)
                {
                    aggregateBounds.Top = bounds.Top;
                }

                if (bounds.Left < aggregateBounds.Left)
                {
                    aggregateBounds.Left = bounds.Left;
                }

                if (bounds.Bottom < aggregateBounds.Bottom)
                {
                    aggregateBounds.Bottom = bounds.Bottom;
                }

                if (bounds.Right > aggregateBounds.Right)
                {
                    aggregateBounds.Right = bounds.Right;
                }
            }
        }

        return aggregateBounds;
    }
}