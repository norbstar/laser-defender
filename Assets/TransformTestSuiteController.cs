using System.Collections.Generic;

using UnityEngine;

public class TransformTestSuiteController : MonoBehaviour
{
    private IList<IAcutationTest> actuators;

    void Awake()
    {
        actuators = new List<IAcutationTest>();
    }

    // Start is called before the first frame update
    void Start()
    {
        actuators = ResolveActuators(transform);
    }

    private IList<IAcutationTest> ResolveActuators(Transform transform)
    {
        List<IAcutationTest> actuators = new List<IAcutationTest>();

        foreach (Transform childTransform in transform)
        {
            var acutationTest = childTransform.GetComponent<IAcutationTest>() as IAcutationTest;

            if (acutationTest != null)
            {
                actuators.Add(acutationTest);
            }

            actuators.AddRange(ResolveActuators(childTransform));
        }

        return actuators;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (IAcutationTest actuator in actuators)
            {
                actuator.Actuate();
            }
        }
    }
}