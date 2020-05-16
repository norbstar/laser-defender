using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Wave Configuration")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] GameObject enemyPrefab; 
    [SerializeField] GameObject pathPrefab;
    [SerializeField] AnimationClip animationClip;
    [SerializeField] float timeBetweenSpawns = 1.0f;
    [SerializeField] int waveSize = 1;
    [SerializeField] float timeBetweenWaves = 1.0f;
    [SerializeField] float speed = 2.0f;
    [SerializeField] bool invertX = false, invertY = false;
    [SerializeField] bool enableBuffeting = false;

    public GameObject GetEnemyPrefab()
    {
        return enemyPrefab;
    }

    public GameObject GetPathPrefab()
    {
        return pathPrefab;
    }

    public IList<Transform> GetWaypoints()
    {
        var waypoints = new List<Transform>();

        foreach (Transform childTransform in pathPrefab.transform)
        {
            waypoints.Add(childTransform);
        }
         
        return waypoints;
    }

    public AnimationClip GetAnimationClip()
    {
        return animationClip;
    }

    public float GetTimeBetweenSpawns()
    {
        return timeBetweenSpawns;
    }

    public int GetWaveSize()
    {
        return waveSize;
    }

    public float GetTimeBetweenWaves()
    {
        return timeBetweenWaves;
    }

    
    public float GetSpeed()
    {
        return speed;
    }

    public bool GetEnableBuffeting()
    {
        return enableBuffeting;
    }

    public bool GetInvertX()
    {
        return invertX;
    }

    public bool GetInvertY()
    {
        return invertY;
    }
}