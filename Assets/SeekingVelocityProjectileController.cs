using System;
using System.Collections;

using UnityEngine;

public class SeekingVelocityProjectileController : SignatureMonoBehaviour, IActuate
{
    [Header("Config")]
    [SerializeField] float speed = 1.0f;
    [Range(0.0f, 50.0f)]
    [SerializeField] float turnSpeed = 10.0f;
    // [SerializeField] float lifespanSecs = 3.5f;

    public class Configuration : GameplayConfiguration
    {
        public GameObject Target { get; set; }
    }

    private new Rigidbody2D rigidbody;
    private GameObject target;
    private RenderLayer layer;

    public override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Actuate(IConfiguration configuration)
    {
        if (configuration != null)
        {
            if (typeof(GameplayConfiguration).IsInstanceOfType(configuration))
            {
                layer = ((GameplayConfiguration) configuration).Layer;
            }

            if (typeof(Configuration).IsInstanceOfType(configuration))
            {
                target = ((Configuration) configuration).Target;
            }
        }

        StartCoroutine(Co_Actuate());
    }

    private void UpdateVelocity()
    {
        // var direction = VectorFunctions.ToVector2(target.transform.position) - VectorFunctions.ToVector2(transform.position);
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0.0f, 0.0f, direction), Time.deltaTime * turnSpeed * 45.0f); 

        // Debug.Log($"Co_Actuate Target: {target.name}");
        var localAngle = (float) Math.Round(transform.rotation.eulerAngles.z);
        // Debug.Log($"Co_Actuate Local Angle: {localAngle}");
        var relativeAngle = MathFunctions.TrueAngle((float) Math.Round(MathFunctions.GetAngle(transform.position, target.transform.position, -90)));
        // Debug.Log($"Co_Actuate Relative Angle: {relativeAngle}");
        var differentialAngle = relativeAngle - localAngle;
        // Debug.Log($"Co_Actuate Differential Angle: {differentialAngle}");
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0.0f, 0.0f, localAngle + differentialAngle), Time.deltaTime * turnSpeed * 45.0f);
        rigidbody.velocity = transform.up * speed;
    }

    private IEnumerator Co_Actuate()
    {
        gameObject.layer = (int) layer;

        var sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        // rigidbody.velocity = new Vector2(0.0f, 1.0f) * speed;
        // rigidbody.velocity = transform.up * speed;

        // if (target == null) yield break;

        // var startTime = Time.time;
        // var endTime = startTime + lifespanSecs;

        while (gameObject != null && target != null/* && Time.time < endTime*/)
        {
            UpdateVelocity();
            yield return null;
        }
    }

    private bool SharesSameLayer(GameObject obj) => obj.layer == gameObject.layer;

    // private bool SharesSameTag(GameObject obj) => obj.tag.Equals(gameObject.tag);

    public void OnTriggerEnter2D(Collider2D collider)
    {
        // bool destroyObject = true;

        // if (gameObject.layer != collider.gameObject.layer || collider.gameObject.tag.Equals(gameObject.tag))
        // {
        //     destroyObject = false;
        // }

        // var baseMonoBehaviour = collider.gameObject.GetComponent<BaseMonoBehaviour>();

        // if (baseMonoBehaviour != null)
        // {
        //     if (gameObject.name.Contains(baseMonoBehaviour.Signature))
        //     {
        //         destroyObject = false;
        //     }
        // }

        // if (SharesSameLayer(collider.gameObject)) return;

        if (SharesSameSignature(collider.gameObject)) return;

        Destroy(gameObject);

        // bool destroyObject = false;

        // if (SharesSameLayer(collider.gameObject) && !SharesSameTag(collider.gameObject))
        // {
        //     destroyObject = true;
        // }

        // if (destroyObject)
        // {
        //     Destroy(gameObject);
        // }
    }
}