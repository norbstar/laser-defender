using UnityEngine;

public class ParticalEffectController : MonoBehaviour, IActuate
{
    [Header("Particle System")]
    [SerializeField] GameObject particalSystemPrefab;
    [SerializeField] bool enableParticalEffects = false;

    void Awake()
    {
        ResolveComponents();
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration = null)
    {
        if (enableParticalEffects)
        {
            var particalSystem = Instantiate(particalSystemPrefab, gameObject.transform.position, particalSystemPrefab.transform.rotation) as GameObject;
            particalSystem.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.x) * 0.5f;

            Destroy(particalSystem, 1.0f);
        }
    }
}