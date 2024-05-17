using System.Collections;

using UnityEngine;

[RequireComponent(typeof(DamageAttributes))]
public class ProjectileController : MonoBehaviour, IActuate
{
    public enum Type
    {
        LIGHT_BULLET,
        LIGHT_PROTON,
        MEDIUM_BULLET,
        MEDIUM_PROTON,
        HEAVY_BULLET,
        HEAVY_PROTON
    }

    [SerializeField] float speed = 1.0f;

    public void Actuate(IConfiguration configuration)
    {
        StartCoroutine(LaunchProjectileCoroutine());
    }

    private IEnumerator LaunchProjectileCoroutine()
    {
        Vector3 originPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, InGameManagerOld.ScreenRatio.y, transform.position.z);
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) * Time.deltaTime * speed;
            transform.position = Vector3.Lerp(originPosition, targetPosition, (float) fractionComplete);

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnJourneyComplete();
            }

            yield return null;
        }
    }

    private void OnJourneyComplete() { }
}