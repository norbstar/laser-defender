using System.Collections;

using UnityEngine;

[RequireComponent(typeof(DamageAttributes))]
public class ProjectileController : MonoBehaviour, IActuate
{
    public enum Type
    {
        LIGHT_BULLET,
        LIGHT_PROTON,
        SEEKING_LIGHT_PROTON,
        MEDIUM_BULLET,
        MEDIUM_PROTON,
        SEEKING_MEDIUM_PROTON,
        HEAVY_BULLET,
        HEAVY_PROTON,
        SEEKING_HEAVY_PROTON
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