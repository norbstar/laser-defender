using System;

using UnityEngine;

[CreateAssetMenu(menuName = "Text Pack")]
public class TextPack : ScriptableObject
{
    [Serializable]
    public class TextAsset
    {
        public string text;
        public float textScale = 1.0f;
        public Color textColor = Color.white;
    }

    [SerializeField] TextAsset[] pack;

    public TextAsset[] Pack { get => pack; }
}