using System.Collections;

using UnityEngine;

public class TargetController : GUIMonoBehaviour
{
    [SerializeField] float speed = 1.0f;
    [SerializeField] float offset = 0.5f;

    private Vector3 upOffset, leftPosition, leftUpPosition, rightPosition, rightUpPosition;

    void Awake()
    {
        ConfigGUIStyle();

        guiStyle.fontSize = 8;
        guiStyle.font = (Font) Resources.Load("Fonts/Block Stock");
        guiStyle.normal.textColor = Color.white;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        PlotPoints();
        yield return StartCoroutine(Co_StartJourney());
    }

    private IEnumerator Co_StartJourney()
    {
        while (true)
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed * Time.deltaTime;

            if (input != Vector2.zero)
            {
                if (input.x != 0)
                {
                    transform.Rotate(new Vector3(0.0f, 0.0f, -input.x * 15.0f), Space.Self);
                }

                if (input.y != 0)
                {
                    transform.position += transform.up * input.y;
                }

                PlotPoints();
            }

            yield return null;
        }
    }

    private void PlotPoints()
    {
        upOffset = transform.position + (transform.up * offset);

        Vector3 leftVector = Vector3.Cross(transform.up, -Vector3.forward).normalized;
        leftPosition = transform.position + leftVector * offset;
        
        Vector3 leftUpVector = Vector3.Cross(-transform.right, Vector3.forward).normalized;
        leftUpPosition = leftPosition + leftUpVector * offset;

        Vector3 rightVector = Vector3.Cross(transform.up, Vector3.forward).normalized;
        rightPosition = transform.position + rightVector * offset;

        Vector3 rightUpVector = Vector3.Cross(transform.right, Vector3.forward).normalized;
        rightUpPosition = rightPosition + leftUpVector * offset;
    }

    private IEnumerator Co_StartFixedJourney()
    {
        Vector3 origin = transform.position;
        Vector3 target = origin + transform.up * 1.0f;
        float startTime = Time.time;
        bool complete = false;
        float speed = 1.0f;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, -45.0f);

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) * speed;
            transform.position = Vector3.Lerp(origin, target, (float)fractionComplete);

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnJourneyComplete();
            }

            yield return null;
        }
    }

    private void OnJourneyComplete() { }

#if (false)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.05f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(upOffset, 0.05f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(leftPosition, 0.05f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(leftUpPosition, 0.05f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(rightPosition, 0.05f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rightUpPosition, 0.05f);
    }
#endif

    public override void SetGUIAttributes(GUIStyle guiStyle)
    {
        GUI.Label(new Rect(20, 25, 200, 40), $"Position: {transform.position}", guiStyle);
        GUI.Label(new Rect(20, 50, 200, 40), $"Rotation: {transform.rotation.eulerAngles}", guiStyle);
        GUI.Label(new Rect(20, 75, 200, 40), $"Up Offset: {upOffset}", guiStyle);
        GUI.Label(new Rect(20, 100, 200, 40), $"Left Position: {leftPosition}", guiStyle);
        GUI.Label(new Rect(20, 125, 200, 40), $"Left Up Position: {leftUpPosition}", guiStyle);
        GUI.Label(new Rect(20, 150, 200, 40), $"Right Position: {rightPosition}", guiStyle);
        GUI.Label(new Rect(20, 175, 200, 40), $"Right Up Position: {rightUpPosition}", guiStyle);
    }
}