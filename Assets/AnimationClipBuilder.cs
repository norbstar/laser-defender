using System.Collections.Generic;

using UnityEngine;

public class AnimationClipBuilder : MonoBehaviour
{
    private IDictionary<string, AnimationCurve> animationCurves = new Dictionary<string, AnimationCurve>();

    public AnimationClipBuilder AddCurve(string key, AnimationCurve animationCurve)
    {
        if (IncludeCurve(animationCurve))
        {
            animationCurves.Add(key, animationCurve);
        }

        return this;
    }

    public AnimationClipBuilder RemoveCurve(string key)
    {
        if (animationCurves.ContainsKey(key))
        {
            animationCurves.Remove(key);
        }

        return this;
    }

    public IDictionary<string, AnimationCurve> GetCurves()
    {
        return animationCurves;
    }

    public AnimationClip CreateClip()
    {
        var clip = new AnimationClip();
        string relativeObjectName = string.Empty;

        foreach (KeyValuePair<string, AnimationCurve> keyPair in animationCurves)
        {
            clip.SetCurve(relativeObjectName, typeof(Transform), keyPair.Key, keyPair.Value);
        }

        return clip;
    }

    private bool IncludeCurve(AnimationCurve animationCurve)
    {
        Keyframe[] keyFrames = animationCurve.keys;

        float valueSum = 0.0f;

        for (int itr = 0; itr < keyFrames.Length; ++itr)
        {
            valueSum += keyFrames[itr].value;
        }

        return (valueSum != 0.0f);
    }
}