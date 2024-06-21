using System.Collections;

using UnityEngine;

public class VelocityProjectileController : MonoBehaviour, IActuate
{
    [SerializeField] float speed = 1.0f;

    public class Configuration : GameplayConfiguration
    {
        public Vector2 Direction { get; set; }
    }

    private new Rigidbody2D rigidbody;
    private Vector2 direction;
    private RenderLayer layer;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents()
    {
        rigidbody = GetComponent<Rigidbody2D>() as Rigidbody2D;
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

    private IEnumerator Co_Actuate()
    {
        gameObject.layer = (int) layer;

        var sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        rigidbody.velocity = /*direction*/ new Vector2(0.0f, 1.0f) * speed;
        yield return null;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        bool destroyObject = true;

        if (gameObject.layer != collider.gameObject.layer)
        {
            destroyObject = false;
        }

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
}