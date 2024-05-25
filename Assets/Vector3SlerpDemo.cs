using System.Collections;

using UnityEngine;

public class Vector3SlerpDemo : MonoBehaviour
{
    [SerializeField] Transform sunrise;
    [SerializeField] Transform sunset;

    [Range(0.0f, 1.0f)]
    [SerializeField] float fractionComplete;

    // Time to move from sunrise to sunset position, in seconds.
    [SerializeField] float journeyTime = 1.0f;

    // The time at which the animation started.
    private float startTime;

    void Start()
    {
        // Note the time at the start of the animation.
        startTime = Time.time;

        StartCoroutine(Co_ITweenTest());
    }

#if (false)
    void Update()
    {
        // The center of the arc
        Vector3 center = (sunrise.position + sunset.position) * 0.5f;

        // move the center a bit downwards to make the arc vertical
        center -= new Vector3(0.0f, 1.0f, 0.0f);

        // Interpolate over the arc relative to center
        Vector3 riseRelCenter = sunrise.position - center;
        Vector3 setRelCenter = sunset.position - center;

        // The fraction of the animation that has happened so far is
        // equal to the elapsed time divided by the desired time for
        // the total journey.
        float fractionComplete = (Time.time - startTime) / journeyTime;

        Vector3 newPosition = Vector3.Slerp(riseRelCenter, setRelCenter, fractionComplete);
        newPosition += center;

        //Vector3 vector = (newPosition - transform.position).normalized;
        //Quaternion rotation = Quaternion.LookRotation(Vector3.forward, vector);
        //transform.rotation = rotation;

        transform.rotation = MathFunctions.AlignZRotationToVector(transform.rotation, transform.position, newPosition, -90);
        transform.position = newPosition;
    }
#endif

    private IEnumerator Co_ITweenTest()
    {
        //iTween.MoveBy(gameObject, iTween.Hash("x", 2, "easeType", "easeInOutExpo", "loopType", "pingPong", "delay", .1));
        //iTween.RotateBy(gameObject, iTween.Hash("z", -1, "easeType", "easeInOutBack", "loopType", "pingPong", "delay", .4));
        yield return null;
    }
}