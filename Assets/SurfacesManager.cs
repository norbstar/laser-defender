using UnityEngine;

public class SurfacesManager : MonoBehaviour
{
    [SerializeField] SurfaceManager[] surfaceManagers;

    [Range(0.0f, 10.0f)]
    [SerializeField] float speed = 1.0f;

    private float lastSpeed;

    // Update is called once per frame
    void Update()
    {
        if (speed != lastSpeed)
        {
            surfaceManagers = GetComponentsInChildren<SurfaceManager>() as SurfaceManager[];

            if (surfaceManagers != null)
            {
                foreach (SurfaceManager surfaceManager in surfaceManagers)
                {
                    float scrollSpeed = surfaceManager.GetDefaultScrollSpeed() * speed;
                    surfaceManager.SetScrollSpeed(scrollSpeed);
                }
            }

            lastSpeed = speed;
        }
    }
}