using System.Collections;

public class LayerActuator : BaseMonoBehaviour, IActuate
{
    private RenderLayer layer;

    public override void Awake()
    {
        base.Awake();
    }

    public void Actuate(IConfiguration configuration)
    {
        if (configuration != null)
        {
            if (typeof(GameplayConfiguration).IsInstanceOfType(configuration))
            {
                layer = ((GameplayConfiguration) configuration).Layer;
            }
        }

        StartCoroutine(Co_Actuate());
    }

    private IEnumerator Co_Actuate()
    {
        gameObject.layer = (int) layer;

        int sortingOrderId = GameObjectFunctions.GetSortingOrderId(layer);
        GameObjectFunctions.DesignateSortingLayer(gameObject, sortingOrderId);

        yield return null;
    }
}