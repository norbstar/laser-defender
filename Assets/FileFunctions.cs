﻿using UnityEngine;

public class FileFunctions : MonoBehaviour
{
    public class Path
    {
        public static string Animations { get; set; } = "Assets/Resources/Animations";
        public static string Audio { get; set; } = "Assets/Resources/Audio";
        public static string Images { get; set; } = "Assets/Resources/Images";
        public static string Materials { get; set; } = "Assets/Resources/Materials";
        public static string Prefabs { get; set; } = "Assets/Resources/Prefabs";
        public static string Sprites { get; set; } = "Assets/Resources/Sprites";
        public static string Textures { get; set; } = "Assets/Resources/Textures";
    }

    public static string CreatePath(string path, string filename)
    {
        return $"{path}/{filename}";
    }
}