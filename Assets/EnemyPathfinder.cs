using System.Collections.Generic;

using UnityEngine;

public class EnemyPathfinder : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] WaveConfig waveConfig;

    [SerializeField] float speed = 1.0f;

    private IList<Transform> waypoints;
    private int waypointItr = 0;

    // Start is called before the first frame update
    void Start()
    {
        waypoints = waveConfig.GetWaypoints();
        transform.position = waypoints[waypointItr].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (waypointItr < waypoints.Count)
        {
            //transform.position = Vector2.MoveTowards(VectorFunctions.ToVector2(transform.position), waypoints[waypointItr].position, Time.deltaTime * speed);
            transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointItr].position, Time.deltaTime * speed);

            if (transform.position == waypoints[waypointItr].position)
            {
                ++waypointItr;
            }
        }
        else
        { 
            Destroy(gameObject);
        }
    }
}