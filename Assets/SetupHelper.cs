using UnityEngine;

public class SetupHelper : MonoBehaviour
{
    public static ISetup SetupManager;

    public static void SetSetupManager(ISetup setupManager)
    {
        SetupManager = setupManager;
    }
}