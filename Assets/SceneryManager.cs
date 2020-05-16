using UnityEngine;

public class SceneryManager : MonoBehaviour
{
    [SerializeField] AbstractSceneryManager[] sceneryElements;

    [Range(0.0f, 10.0f)]
    [SerializeField] float speed = 1.0f;

    private float lastSpeed;

    public void Awake()
    {
        lastSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (speed != lastSpeed)
        {
            sceneryElements = GetComponentsInChildren<AbstractSceneryManager>() as AbstractSceneryManager[];

            if (sceneryElements != null)
            {
                foreach (AbstractSceneryManager sceneryElement in sceneryElements)
                {
                    float scrollSpeed = sceneryElement.GetReferenceScrollSpeed() * speed;
                    sceneryElement.SetScrollSpeed(scrollSpeed);
                }
            }

            lastSpeed = speed;
        }
    }
}