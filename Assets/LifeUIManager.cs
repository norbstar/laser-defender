using UnityEngine;

public class LifeUIManager : MonoBehaviour
{
    [SerializeField] GameObject activeLife, expendedLife;

    public enum State
    {
        ACTIVE,
        EXPANDED
    }

    private State state;

    // Start is called before the first frame update
    void Start()
    {
        ResetState();
    }

    public void SetState(State state)
    {
        this.state = state;

        switch (state)
        {
            case State.ACTIVE:
                activeLife?.SetActive(true);
                expendedLife?.SetActive(false);
                break;

            case State.EXPANDED:
                activeLife?.SetActive(false);
                expendedLife?.SetActive(true);
                break;
        }
    }

    public void ResetState()
    {
        SetState(State.ACTIVE);
    }

    public State GetState()
    {
        return state;
    }
}