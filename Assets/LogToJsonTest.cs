using System;
using System.Collections.Generic;

using UnityEngine;

public class LogToJsonTest : MonoBehaviour
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
    }

    [Serializable]
    public class WaveData
    {
        public List<Wave> waves;
    }

    // Start is called before the first frame update
    void Start()
    {
        WaveData payload = new WaveData
        {
            waves = new List<Wave>
            {
                new Wave
                {
                    id = 1,
                    spawnCount = 3,
                    logs = new List<Log>
                    {
                        new Log
                        {
                            timestamp = "0.0000",
                            delta = null,
                            message = "Start"
                        },
                        new Log
                        {
                            timestamp = "7.1135",
                            delta = "1.0156",
                            message = "End"
                        }
                    }
                },
                new Wave
                {
                    id = 2,
                    spawnCount = 5,
                    logs = new List<Log>
                    {
                    }
                }
            }
        };

        //Wave payload = new Wave
        //{
        //    id = 1,
        //    spawnCount = 3,
        //    logs = new List<Log>
        //    {
        //        new Log
        //        {
        //            timestamp = "0.0000",
        //            delta = null,
        //            message = "Start"
        //        },
        //        new Log
        //        {
        //            timestamp = "7.1135",
        //            delta = "1.0156",
        //            message = "End"
        //        }
        //    }
        //};

        string json = JsonUtility.ToJson(payload);
        LogFunctions.LogRawToFile($"wave.json", json);
        Wave retrievedPayload = JsonUtility.FromJson<Wave>(json);
        Debug.Log(retrievedPayload);
    }
}