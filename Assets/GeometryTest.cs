using UnityEngine;

public class GeometryTest : MonoBehaviour
{
    private GeometryFunctions.Bounds bounds;

    // Start is called before the first frame update
    void Start()
    {
        bounds = GeometryFunctions.GetBounds(gameObject);
    }

#if (false)
    void OnDrawGizmos()
    {
        if (bounds != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(transform.position.x, bounds.Top, transform.position.z), new Vector3(transform.position.x, bounds.Bottom, transform.position.z));
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(bounds.Left, transform.position.y, transform.position.z), new Vector3(bounds.Right, transform.position.y, transform.position.z));
        }
    }
#endif
}