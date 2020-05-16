using UnityEngine;

public class BossAcuationManagerDemo : MonoBehaviour
{
    [SerializeField] BossActuationManager bossActuationManager;

    public enum State
    {
        DEFAULT,
        ACTIVATED
    }

    private State state = State.DEFAULT;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (state)
            {
                case State.DEFAULT:
                    bossActuationManager.Actuate();
                    state = State.ACTIVATED;
                    break;

                case State.ACTIVATED:
                    //bossActuationManager.DebugForceDestruction();
                    state = State.DEFAULT;
                    break;
            }
        }
    }
}