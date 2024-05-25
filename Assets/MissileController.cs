using System.Collections;

using UnityEngine;

public class MissileController : MonoBehaviour, IActuate
{
    [Range(0.0f, 10.0f)]
    [SerializeField] float speed = 1.0f;

    [Range(0.0f, 10.0f)]
    [SerializeField] float turnSpeed = 10.0f;

    public class Configuration : GameplayConfiguration
    {
        public GameObject Target { get; set; }
    }

    private new Rigidbody2D rigidbody;
    private GameObject target;
    private float moveForwardOffset = 0.5f;

    void Awake()
    {
        ResolveComponents();

        /*
        Vector2.Angle
        Vector2.SignedAngle
        Vector2.Distance
        Vector2.MoveTowards
        Vector2.Dot

        Vector3.Angle
        Vector3.SignedAngle
        Vector3.AngleBetween
        Vector3.Distance
        Vector3.RotateTowards
        Vector3.MoveTowards
        Vector3.Dot

        Quaternion.LookRotation
        Quaternion.Angle
        Quaternion.AngleAxis
        Quaternion.AxisAngle
        Quaternion.EulerRotation
        Quaternion.FromToRotation
        Quaternion.LookRotation
        Quaternion.RotateTowards
        Quaternion.Dot
        */
    }

    private void ResolveComponents()
    {
        rigidbody = GetComponent<Rigidbody2D>() as Rigidbody2D;
    }

    public void Actuate(IConfiguration configuration)
    {
        if (typeof(Configuration).IsInstanceOfType(configuration))
        {
            target = ((Configuration) configuration).Target;
        }

        StartCoroutine(Co_Actuate());
    }

    private IEnumerator Co_Actuate()
    {
        yield return StartCoroutine(Co_MoveForward());
    }

    private IEnumerator Co_MoveForward()
    {
        Vector3 originPosition = transform.position;
        Vector3 targetPosition = (transform.position + (Vector3.up * moveForwardOffset));
        //float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            //float fractionComplete = (Time.time - startTime) * speed;
            var fractionComplete = (targetPosition - originPosition).magnitude * Time.deltaTime * speed;
            transform.position = Vector3.Lerp(transform.position, targetPosition, fractionComplete);

            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);

            //float distance = Vector3.Distance(transform.position, targetPosition);
            //float finalSpeed = (distance / speed);
            //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / finalSpeed);

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnMoveForwardComplete();
            }

            yield return null;
        }
    }

    private IEnumerator Co_HomeOnTarget()
    {
        Vector3 originPosition = transform.position;
        Vector3 targetPosition = target.transform.position;
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * speed;
            transform.position = Vector3.Lerp(originPosition, targetPosition, fractionComplete);
            //transform.rotation = MathFunctions.AlignZRotationToVector(transform.rotation, transform.position, transform.up, 90);

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnHomeOnTargetComplete();
            }

            yield return null;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        bool destroyObject = true;

        var baseMonoBehaviour = collider.gameObject.GetComponent<BaseMonoBehaviour>() as BaseMonoBehaviour;

        if (baseMonoBehaviour != null)
        {
            if (gameObject.name.Contains(baseMonoBehaviour.Signature))
            {
                destroyObject = false;
            }
        }

        if (destroyObject)
        {
            Destroy(gameObject);
        }
    }

    private void OnMoveForwardComplete()
    {
        StartCoroutine(Co_HomeOnTarget());
    }

    private void OnHomeOnTargetComplete() { }
}