using UnityEngine;

public class Testbed : MonoBehaviour
{
    [SerializeField] float fractionComplete;
    [SerializeField] float originTimestamp, targetTimestamp;
    [SerializeField] float origin, target;
    [SerializeField] float offset, delta;
    [SerializeField] float interFractionComplete;
    [SerializeField] float result;

    // Update is called once per frame
    void Update()
    {
        offset = fractionComplete - originTimestamp;
        interFractionComplete = offset / (targetTimestamp - originTimestamp);
        delta = target - origin;
        result = origin + (delta * interFractionComplete);
    }
}