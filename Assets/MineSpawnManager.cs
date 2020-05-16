using UnityEngine;

public class MineSpawnManager : MonoBehaviour
{
    [SerializeField] GameObject minePrefab;
    [SerializeField] Vector3 position;
    [SerializeField] Vector3 rotation;
    [SerializeField] Vector3 scale = Vector3.one;
    [SerializeField] bool actuate = true;

    // Start is called before the first frame update
    void Start()
    {
        var mine = Instantiate(minePrefab, position, Quaternion.identity) as GameObject;
        mine.transform.rotation = Quaternion.Euler(rotation);
        mine.transform.localScale = scale;

        IActuate mineController = mine.GetComponent<MineController>() as MineController;

        if (mineController == null)
        {
            mineController = mine.GetComponent<ProximityMineController>() as ProximityMineController;
        }

        if (actuate)
        {
            mineController?.Actuate();
        }
    }
}