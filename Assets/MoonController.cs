using UnityEngine;

public class MoonController : GUIMonoBehaviour
{
    [SerializeField] GameObject earth;

    [Range(0.0f, 1.0f)]
    [SerializeField] float fractionComplete = 0.0f;

    private float? lastFractionComplete = null;
    private float distance, angle, relativeAngle;
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
        }

        angle = 360.0f * fractionComplete;

        transform.position = VectorFunctions.ToVector3(MathFunctions.GetPosition(VectorFunctions.ToVector2(earth.transform.position), distance, -angle, 0), originalPosition.z);

        Vector3 direction = transform.position - earth.transform.position;
        relativeAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    public override void SetGUIAttributes(GUIStyle guiStyle)
    {
        GUI.Label(new Rect(20, 25, 200, 40), $"Distance: {distance}", guiStyle);
        GUI.Label(new Rect(20, 50, 200, 40), $"Angle: {angle}", guiStyle);
        GUI.Label(new Rect(20, 75, 200, 40), $"Fraction Complete: {fractionComplete}", guiStyle);
        GUI.Label(new Rect(20, 100, 200, 40), $"X: {transform.position.x}", guiStyle);
        GUI.Label(new Rect(20, 125, 200, 40), $"Y: {transform.position.y}", guiStyle);
        GUI.Label(new Rect(20, 150, 200, 40), $"Relative Angle: {relativeAngle}", guiStyle);
    }

#if (false)
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        Gizmos.DrawLine(transform.position, earth.transform.position);
    }
#endif
}