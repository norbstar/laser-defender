using System.Collections;

using UnityEngine;

[RequireComponent(typeof(DamageAttributes))]
public class ProjectileController : MonoBehaviour, IActuate
{
    public enum Type
    {
        LIGHT_BULLET,
        LIGHT_MISSILE,
        SEEKING_LIGHT_MISSILE,
        MEDIUM_BULLET,
        MEDIUM_MISSILE,
        SEEKING_MEDIUM_MISSILE,
        HEAVY_BULLET,
        HEAVY_MISSILE,
        SEEKING_HEAVY_MISSILE
    }

    public enum Mode
    {
        NORMAL,
        SEEKING
    }

    [SerializeField] float speed = 1f;

    public void Actuate(IConfiguration configuration)
    {
        StartCoroutine(Co_LaunchProjectile());
    }

    private IEnumerator Co_LaunchProjectile()
    {
        Vector3 originPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, InGameManagerOld.ScreenRatio.y, transform.position.z);
        float startTime = Time.time;
        bool complete = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * Time.deltaTime * speed;
            transform.position = Vector3.Lerp(originPosition, targetPosition, fractionComplete);

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnJourneyComplete();
            }

            yield return null;
        }
    }

    private void OnJourneyComplete() { }
}