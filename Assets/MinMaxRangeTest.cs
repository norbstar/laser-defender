using System;

using UnityEngine;

public class MinMaxRangeTest : MonoBehaviour
{
    public enum SettingType
    {
        INTER_SPAWN_DELAY,
        SPEED,
        ROTATION,
        SCALE,
        SPAWN_COUNT
    }

    [Serializable]
    public class MinMaxSettings
    {
        public float minValue;
        [System.NonSerialized]
        public float minLimit;
        public float maxValue;
        [System.NonSerialized]
        public float maxLimit;
        public MinMaxRange minMaxRange;

        public MinMaxSettings()
        {
            minValue = 5;
            minLimit = 0;
            maxValue = 10;
            maxLimit = 15;

            minMaxRange.minValue = minValue;
            minMaxRange.min = minLimit;
            minMaxRange.maxValue = maxValue;
            minMaxRange.max = maxLimit;
        }
    }

    //[Serializable]
    //public class SpawnSetting
    //{
    //    public SettingType settingType;
    //    public MinMaxSettings settings;
    //}

    [Serializable]
    public class SpawnSettings1
    {
        //public SpawnSetting[] settings;

        public MinMaxSettings interspawnDelay;
        //public MinMaxSettings speed;
        //public MinMaxSettings rotation;
        //public MinMaxSettings scale;
        //public MinMaxSettings spawnCount;
    }

    [SerializeField] SpawnSettings1 spawnSettings1;
    //[SerializeField] MinMaxRange minMaxRange;
}