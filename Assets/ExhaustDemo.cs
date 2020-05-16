using UnityEngine;

public class ExhaustDemo : GUIMonoBehaviour
{
    [SerializeField] GameObject exhaust;

    [Range(0.0f, 1.0f)]
    [SerializeField] float thrustRatio = 0.5f;

    private DynamicExhaustController dynamicExhaustController;
    private Vector2 input;
    //private float ratio;

    void Awake()
    {
        ResolveComponents();
        ConfigGUIStyle();

        guiStyle.fontSize = 14;
        guiStyle.font = (Font) Resources.Load("Fonts/Block Stock");
        guiStyle.normal.textColor = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        dynamicExhaustController.SetThrust(input.y);

        //input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime;
        //ratio = MathFunctions.GetRelativeRatio(-0.16f * Time.deltaTime, 0.16f * Time.deltaTime, input.y);
        //thrustRatio = MathFunctions.GetValueInRange(0.0f, 1.0f, ratio);

        //dynamicExhaustController.SetThrust(thrustRatio);
    }

    private void ResolveComponents()
    {
        dynamicExhaustController = exhaust.GetComponent<DynamicExhaustController>() as DynamicExhaustController;
    }

    public override void SetGUIAttributes(GUIStyle guiStyle)
    {
        GUI.Label(new Rect(20, 25, 200, 40), $"Input Y: {input.y}", guiStyle);
        //GUI.Label(new Rect(20, 50, 200, 40), $"Ratio: {ratio}", guiStyle);
        GUI.Label(new Rect(20, 75, 200, 40), $"Thrust Ratio: {thrustRatio}", guiStyle);
    }
}