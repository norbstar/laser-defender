using System;

using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T) Activator.CreateInstance(typeof(T));
            }

            return instance;
        }
    }
}
