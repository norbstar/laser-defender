using System.Collections;

using UnityEngine;

public class SpinAnimator : MonoBehaviour
{
    [SerializeField] float speed = 1.0f;

    IEnumerator Start()
    {
        while (true)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * Quaternion.Euler(0.0f, 0.0f, 1.0f), Time.deltaTime * speed);
            yield return null;
        }
    }
}