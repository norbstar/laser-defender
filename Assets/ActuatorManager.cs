using System.Collections.Generic;

using UnityEngine;

public class ActuatorManager : MonoBehaviour// SingletonMonoBehaviour<ActuatorManager>
{
    [Header("Analytics")]
    [SerializeField] int count;

    // private static ActuatorManager instance;
    private List<GameObject> actuators;

    // private static List<GameObject> collection = new List<GameObject>();

    // public static void Add(GameObject gameObject) => collection.Add(gameObject);

    // public static void Remove(GameObject gameObject) => collection.Remove(gameObject);

    void Awake()
    {
        // instance = this;
        actuators = new List<GameObject>();
    }

    // public static ActuatorManager Instance { get => instance; }

    public void Add(GameObject gameObject) => actuators.Add(gameObject);

    public void Remove(GameObject gameObject) => actuators.Remove(gameObject);

    public List<GameObject> Actuators => actuators;

    // Update is called once per frame
    void Update()
    {
        // for (int itr = collection.Count - 1; itr >= 0; --itr)
        // {
        //     var item = collection[itr];

        //     if (item == null)
        //     {
        //         collection.RemoveAt(itr);
        //     }
        // }

        // actuators = collection;

        for (int itr = actuators.Count - 1; itr >= 0; --itr)
        {
            var item = actuators[itr];

            if (item == null)
            {
                actuators.RemoveAt(itr);
            }
        }

        count = actuators.Count;
    }
}
