using System;
using System.Collections.Generic;

using UnityEngine;

public abstract class WaveDataMonoBehaviour : MonoBehaviour
{
    [Serializable]
    public class Log
    {
        public string timestamp;
        public string delta;
        public string message;
    }

    [Serializable]
    public class Wave
    {
        public int id;
        public int spawnCount;
        public List<Log> logs;

        public Wave()
        {
            logs = new List<Log>();
        }
    }

    [Serializable]
    public class WaveData
    {
        public List<Wave> waves;

        public WaveData()
        {
            waves = new List<Wave>();
        }
    }

    public abstract string GetTag();

    private WaveData waveData;
    private Wave wave;
    private float? lastTimestamp;

    protected void AddLogEntry(Wave wave, string message)
    {
        Log log = new Log
        {
            timestamp = Time.time.ToString(),
            message = message
        };

        if (lastTimestamp == null)
        {
            log.delta = null;
        }
        else
        {
            log.delta = (Time.time - lastTimestamp.Value).ToString();
        }

        wave.logs.Add(log);
    }

    protected Wave RetrieveWave(int waveSequence)
    {
        foreach (Wave wave in waveData.waves)
        {
            if (wave.id == waveSequence)
            {
                return wave;
            }
        }

        return null;
    }

    void OnDestroy()
    {
        string json = JsonUtility.ToJson(waveData);
        LogFunctions.LogRawToFile($"{GetTag()}.json", json);
    }
}