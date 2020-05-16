using UnityEngine;

public class MathFunctionsDemo : MonoBehaviour
{
    [SerializeField] float angle;
    [SerializeField] float trueAngle;
    [SerializeField] float relativeAngle;
    [SerializeField] float adjustByAngle;
    [SerializeField] float adjustedTrueAngle;

    // Update is called once per frame
    void Update()
    {
        trueAngle = MathFunctions.TrueAngle(angle);
        relativeAngle = MathFunctions.RelativisticAngle(trueAngle);
        adjustedTrueAngle = MathFunctions.ModifyTrueAngle(angle, adjustByAngle);
    }
}