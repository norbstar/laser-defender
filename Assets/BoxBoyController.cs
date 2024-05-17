using System.Collections;

using UnityEngine;

public class BoxBoyController : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    [Range(0.0f, 10.0f)]
    [SerializeField] float duration = 1.0f;

    private bool acceptInput;

    // Start is called before the first frame update
    void Start()
    {
        //Vector3 offset = transform.TransformPoint(new Vector3(0.5f, -0.5f, 0.0f));

        //var pivotPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //pivotPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //pivotPoint.transform.position = offset;
        //pivotPoint.name = "Pivot Point";

        //var pivotPoint = Instantiate(prefab, transform.position + new Vector3(0.5f, -0.5f, 0.0f), Quaternion.identity) as GameObject;
        //pivotPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //pivotPoint.transform.position = offset;
        //pivotPoint.name = "Pivot Point";

        //transform.parent = pivotPoint.transform;

        acceptInput = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (acceptInput)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                //Debug.Log($"Move Left");

                var pivotPoint = Instantiate(prefab, transform.position + new Vector3(-0.5f, -0.5f, 0.0f), Quaternion.identity) as GameObject;
                pivotPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //pivotPoint.transform.position = transform.TransformPoint(new Vector3(-0.5f, -0.5f, 0.0f));
                pivotPoint.name = "Pivot Point";

                transform.parent = pivotPoint.transform;

                Quaternion rotation = pivotPoint.transform.localRotation * Quaternion.Euler(0.0f, 0.0f, 90.0f);
                StartCoroutine(RotateCoroutine(pivotPoint, rotation));
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                //Debug.Log($"Move Right");

                var pivotPoint = Instantiate(prefab, transform.position + new Vector3(0.5f, -0.5f, 0.0f), Quaternion.identity) as GameObject;
                pivotPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                //pivotPoint.transform.position = transform.TransformPoint(new Vector3(0.5f, -0.5f, 0.0f));
                pivotPoint.name = "Pivot Point";

                transform.parent = pivotPoint.transform;

                Quaternion rotation = pivotPoint.transform.localRotation * Quaternion.Euler(0.0f, 0.0f, -90.0f);
                StartCoroutine(RotateCoroutine(pivotPoint, rotation));
            }
        }
    }

    private IEnumerator RotateCoroutine(GameObject pivotPoint, Quaternion targetRotation)
    {
        acceptInput = false;

        float startTime = Time.time;
        bool complete = false;
        Quaternion originRotation = pivotPoint.transform.localRotation;

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / duration;
            //Debug.Log($"Fraction Complete: {fractionComplete}");

            pivotPoint.transform.localRotation = Quaternion.Slerp(originRotation, targetRotation, fractionComplete);

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnComplete(pivotPoint);
            }

            yield return null;
        }
    }

    private void OnComplete(GameObject pivotPoint)
    {
        //Debug.Log($"OnComplete");

        transform.parent = pivotPoint.transform.parent;
        Destroy(pivotPoint);

        acceptInput = true;
    }
}