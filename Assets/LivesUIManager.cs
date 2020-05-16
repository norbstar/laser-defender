using UnityEngine;

public class LivesUIManager : MonoBehaviour
{
    private static string Tag { get; } = "LivesUIManager";

    public LifeUIManager[] lives;

    private int livesRemaining;

    // Start is called before the first frame update
    void Start()
    {
        ResetLives();
    }

    public bool HasLives()
    {
        return (livesRemaining > 0);
    }

    public void RemoveLife()
    {
        if (livesRemaining > 0)
        {
            --livesRemaining;
            SyncLives();
        }
    }

    private void SyncLives()
    {
        int itr = lives.Length - 1;

        foreach (LifeUIManager life in lives)
        {
            bool isFull = livesRemaining >= (itr + 1);

            life.SetState((isFull) ? LifeUIManager.State.ACTIVE : LifeUIManager.State.EXPANDED);
            --itr;
        }
    }

    public void ResetLives()
    {
        foreach (LifeUIManager life in lives)
        {
            life.ResetState();
        }

        livesRemaining = lives.Length;
    }
}