using System.Collections;

using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float turnSpeed = 1.0f;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(RotateCoroutine());
    }

    private IEnumerator RotateCoroutine()
    {
        while (true)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0.0f, 0.0f, 1.0f), turnSpeed * Time.deltaTime);
            yield return null;
        }
    }
}