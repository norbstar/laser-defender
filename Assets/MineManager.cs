using UnityEngine;

public class MineManager : MonoBehaviour
{
    [SerializeField] GameObject[] mines;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject mine in mines)
        {
            IActuate mineController = mine.GetComponent<MineController>() as MineController;

            if (mineController != null)
            {
                mineController?.Actuate();
            }
            else
            {
                mineController = mine.GetComponent<ProximityMineController>() as ProximityMineController;

                if (mineController != null)
                {
                    mineController?.Actuate();
                }
            }
        }
    }
}