using System;

using UnityEngine;

public abstract class CutSceneActuator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject target;

    [Serializable]
    public class AudioSyncPoint
    {
        public AudioClip audioClip;
        public float syncStartTime;
    }

    [SerializeField] AudioSyncPoint[] audioSyncPoints;
}