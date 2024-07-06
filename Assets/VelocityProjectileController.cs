using System.Collections;

using UnityEngine;

public class VelocityProjectileController : SignatureMonoBehaviour, IActuate
{
    [Header("Config")]
    [SerializeField] float speed = 1.0f;

    public class Configuration : GameplayConfiguration
    {
        public Vector2 Direction { get; set; }
    }

    private new Rigidbody2D rigidbody;
    // private Vector2 direction;
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

            // if (typeof(Configuration).IsInstanceOfType(configuration))
            // {
            //     direction = ((Configuration) configuration).Direction;
            // }
        }

        StartCoroutine(Co_Actuate());
    }

    private void ApplyVelocity() => rigidbody.velocity = /*direction*/ new Vector2(0.0f, 1.0f) * speed;

    private IEnumerator Co_Actuate()
    {
        gameObject.layer = (int) layer;

        var sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        ApplyVelocity();
        yield return null;
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

        // var baseMonoBehaviour = collider.gameObject.GetComponent<BaseMonoBehaviour>() as BaseMonoBehaviour;

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