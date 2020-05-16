using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Text Audio Pack")]
public class TextAudioPack : ScriptableObject
{
    [Serializable]
    public class TextAudioAsset
    {
        public string text;
        public AudioClip audioClip;
    }

    [SerializeField] TextAudioAsset[] pack;

    public TextAudioAsset[] GetPack()
    {
        return pack;
    }
}