using System;

using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
    public float min, minValue, max, maxValue;

    public MinMaxAttribute(float min, float minValue, float max, float maxValue)
    {
        this.min = min;
        this.minValue = minValue;
        this.max = max;
        this.maxValue = maxValue;
    }
}

[Serializable]
public struct MinMaxRange
{
    public float min, minValue, max, maxValue;

    public MinMaxRange(float min, float minValue, float max, float maxValue)
    {
        this.min = min;
        this.minValue = minValue;
        this.max = max;
        this.maxValue = maxValue;
    }

    //public float Clamp(float value)
    //{
    //    return Mathf.Clamp(value, min, max);
    //}

    //public float RandomValue
    //{
    //    get { return UnityEngine.Random.Range(min, max); }
    //}
}