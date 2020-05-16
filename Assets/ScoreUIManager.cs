using UnityEngine;

public class ScoreUIManager : MonoBehaviour
{
    [SerializeField] TextUIManager textUIManager;

    private int score;

    public void SupplementScore(int score)
    {
        SetScore(GetScore() + score);
    }

    public void ResetScore()
    {
        SetScore(0);
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int score)
    {
        this.score = score;
        textUIManager.SetText(score.ToString());
    }
}