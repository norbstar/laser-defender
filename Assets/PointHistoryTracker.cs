using UnityEngine;

public class PointHistoryTracker : MonoBehaviour
{
    [SerializeField] GameObject referencePointPrefab;
    [SerializeField] GameObject target;

    private Vector2? lastPosition = null;

    // Update is called once per frame
    void Update()
    {
        Vector2 position = VectorFunctions.ToVector2(target.transform.position);

        if ((!lastPosition.HasValue) || (position != lastPosition))
        {
            Instantiate(referencePointPrefab, position, Quaternion.identity);
            lastPosition = position;
        }
    }
}