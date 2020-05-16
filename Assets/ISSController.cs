using UnityEngine;

public class ISSController : GUIMonoBehaviour
{
    [SerializeField] GameObject earth;

    [Range(0.0f, 1.0f)]
    [SerializeField] float fractionComplete = 0.0f;

    [SerializeField] float rotationSpeed = 0.5f;

    [Range(0.0f, 360.0f)]
    [SerializeField] float issLocalRotation = 0.0f;

    private double? lastFractionComplete = null;
    private float distance, angle, issLocalAngle, issToEarthAngle, issToEarthLeftOffset, issToEarthRightOffset;
    private bool issInRange;
    private Vector3 originalPosition;

    void Awake()
    {
        ResolveComponents();
        ConfigGUIStyle();

        guiStyle.fontSize = 8;
        guiStyle.font = (Font) Resources.Load("Fonts/Block Stock");
        guiStyle.normal.textColor = Color.white;
    }

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        distance = (transform.position - earth.transform.position).magnitude;
    }

    private void ResolveComponents() { }

    // Update is called once per frame
    void Update()
    {
        if ((lastFractionComplete == null) || (fractionComplete != lastFractionComplete.Value))
        {
            lastFractionComplete = fractionComplete;
            //Debug.Log($"Fraction Complete: {fractionComplete}");

            angle = MathFunctions.GetAngle(fractionComplete);
            //Debug.Log($"Angle: {angle}");

            transform.position = VectorFunctions.ToVector3(MathFunctions.GetPosition(VectorFunctions.ToVector2(earth.transform.position), distance, -angle, 0), originalPosition.z);
        }

#if (true)
        //Vector3 direction = transform.position - earth.transform.position;
        //relativeAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        

        //Vector3 issDirection = transform.TransformDirection(Vector3.right);

        issLocalAngle = Mathf.Round(transform.rotation.eulerAngles.z);
        issToEarthAngle = MathFunctions.TrueAngle(Mathf.Round(MathFunctions.GetAngle(transform.position, earth.transform.position, 0)));
        //issToEarthAngle = (issToEarthAngle == 360.0f) ? 0.0f : issToEarthAngle;

        issToEarthLeftOffset = MathFunctions.ModifyTrueAngle(issToEarthAngle, -45.0f);
        issToEarthRightOffset = MathFunctions.ModifyTrueAngle(issToEarthAngle, +45.0f);

        Debug.Log($"Local Angle: {issLocalAngle} To Earth Angle: {issToEarthAngle} To Earth Left Offset {issToEarthLeftOffset} To Earth Right Offset {issToEarthRightOffset}");

        //issToEarthLeftOffset = AdjustAngleWithinRange(issToEarthAngle + 45.0f);
        //issToEarthRightOffset = AdjustAngleWithinRange(issToEarthAngle - 45.0f);

        if (issToEarthLeftOffset > issToEarthRightOffset)
        {
            issInRange = ((issLocalAngle <= issToEarthLeftOffset) && (issLocalAngle >= issToEarthRightOffset));
        }
        else if (issToEarthLeftOffset < issToEarthRightOffset)
        {
            issInRange = ((issLocalAngle >= issToEarthLeftOffset) && (issLocalAngle <= issToEarthRightOffset));
        }
        else
        {
            issInRange = true;
        }

        //issInRange = (issLocalAngle == issToEarthAngle);

        if (issInRange)
        {
            Debug.Log("Fire!");
        }

        //transform.Rotate(Vector3.forward, -rotationSpeed);
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, issLocalRotation);

        fractionComplete += Input.GetAxis("Horizontal") * Time.deltaTime;
#endif
    }

    //private float AdjustAngleWithinRange(float angle)
    //{
    //    if (angle < 0.0f)
    //    {
    //        angle = 360 + angle;
    //    }
    //    else if (angle > 360.0f)
    //    {
    //        angle = 360.0f % angle;
    //    }

    //    return angle;
    //}

    private int labelIndex;

    public override void SetGUIAttributes(GUIStyle guiStyle)
    {
        labelIndex = 0;

        //PublishLabel($"Distance: {distance}");
        //PublishLabel($"Angle: {angle}");
        //PublishLabel($"Fraction Complete: {fractionComplete}");
        //PublishLabel($"X: {transform.position.x}");
        //PublishLabel($"Y: {transform.position.y}");
        //PublishLabel($"Relative Angle: {relativeAngle}");
        //PublishLabel($"ISS Forward: {forward}");
        PublishLabel($"ISS Local Angle: {issLocalAngle}");
        PublishLabel($"ISS To Earth Angle: {issToEarthAngle}");
        PublishLabel($"ISS To Earth Left Offset: {issToEarthLeftOffset}");
        PublishLabel($"ISS To Earth Right Offset: {issToEarthRightOffset}");
        PublishLabel($"ISS In Range: {issInRange}");
    }

    private void PublishLabel(string text = null)
    {
        text = (text != null) ? text : "";
        GUI.Label(new Rect(20, 25 + (25 * labelIndex + 1), 200, 40), text, guiStyle);
        ++labelIndex;
    }

#if (false)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, earth.transform.position);

        Gizmos.color = (issInRange) ? Color.red: Color.grey;
        Vector3 direction = transform.TransformDirection(Vector3.right);
        Gizmos.DrawRay(transform.position, direction);
    }
#endif
}