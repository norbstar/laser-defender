using UnityEngine;

public class AnimationFunctions
{
    public static float StartTime = 0.0f;
    public static float EndTime = 1.0f;

    public AnimationClip CreateSampleClip()
    {
        var animationClip = new AnimationClip();

        AddLinearCurve(animationClip, "localPosition.x", StartTime, EndTime);
        AddLinearCurve(animationClip, "localPosition.x", StartTime, EndTime);
        AddParabolicCurve(animationClip, "localPosition.y", StartTime, EndTime);
        AddSinWaveCurve(animationClip, "localPosition.z", StartTime, EndTime);

        return animationClip;
    }

    public static void AddCustomCurve(AnimationClip animationClip, Keyframe[] keyframes, string propertyName, bool smoothTangents = true)
    {
        AnimationCurve curve = CreateCustomCurve(keyframes, smoothTangents);
        string relativeObjectName = string.Empty;
        animationClip.SetCurve(relativeObjectName, typeof(Transform), propertyName, curve);
    }

    public static AnimationCurve CreateCustomCurve(Keyframe[] keyframes, bool smoothTangents = true)
    {
        AnimationCurve curve = new AnimationCurve(keyframes);

        //if (smoothTangents)
        //{
        //    curve = SmoothTangents(curve);
        //}
        //else
        //{
        //    curve = FlattenTangents(curve);
        //}

        if (smoothTangents)
        {
            curve = SmoothTangents(curve);
        }

        return curve;
    }

    public static void AddLinearCurve(AnimationClip animationClip, string propertyName, float startTime, float endTime, bool smoothTangents = true)
    {
        float startValue = 0.0f;
        float endValue = 1.0f;

        AnimationCurve curve = CreateLinearCurve(startTime, startValue, endTime, endValue, smoothTangents);
        string relativeObjectName = string.Empty;
        animationClip.SetCurve(relativeObjectName, typeof(Transform), propertyName, curve);
    }

    public static AnimationCurve CreateLinearCurve(float timeStart, float valueStart, float timeEnd, float valueEnd, bool smoothTangents = true)
    {
        var keyframes = new Keyframe[]
        {
            new Keyframe(timeStart, valueStart),
            new Keyframe(timeEnd, valueEnd)
        };

        AnimationCurve curve = new AnimationCurve(keyframes);
        return (smoothTangents) ? SmoothTangents(curve) : curve;
    }

    public static void AddParabolicCurve(AnimationClip animationClip, string propertyName, float startTime, float endTime, bool smoothTangents = true)
    {
        float startValue = 0.0f;
        float endValue = 0.0f;

        AnimationCurve curve = CreateParabolicCurve(startTime, startValue, endTime, endValue, smoothTangents);
        string relativeObjectName = string.Empty;
        animationClip.SetCurve(relativeObjectName, typeof(Transform), propertyName, curve);
    }

    public static AnimationCurve CreateParabolicCurve(float timeStart, float valueStart, float timeEnd, float valueEnd, bool smoothTangents = true)
    {
        var keyframes = new Keyframe[]
        {
            new Keyframe(timeStart, valueStart),
            new Keyframe((timeEnd + timeStart) * 0.5f, (valueEnd + valueStart) + 1.0f),
            new Keyframe(timeEnd, valueEnd)
        };

        AnimationCurve curve = new AnimationCurve(keyframes);
        return (smoothTangents) ? SmoothTangents(curve) : curve;
    }

    public static void AddSinWaveCurve(AnimationClip animationClip, string propertyName, float startTime, float endTime, bool smoothTangents = true)
    {
        float startValue = 0.0f;
        float endValue = 0.0f;

        AnimationCurve curve = CreateSinWaveCurve(startTime, startValue, endTime, endValue, smoothTangents);
        string relativeObjectName = string.Empty;
        animationClip.SetCurve(relativeObjectName, typeof(Transform), propertyName, curve);
    }

    public static AnimationCurve CreateSinWaveCurve(float timeStart, float valueStart, float timeEnd, float valueEnd, bool smoothTangents = true)
    {
        var keyframes = new Keyframe[]
        {
            new Keyframe(timeStart, valueStart),
            new Keyframe((timeEnd + timeStart) * 0.25f, (valueEnd + valueStart) + 1.0f),
            new Keyframe((timeEnd + timeStart) * 0.75f, (valueEnd + valueStart) - 1.0f),
            new Keyframe(timeEnd, valueEnd)
        };

        AnimationCurve curve = new AnimationCurve(keyframes);
        return (smoothTangents) ? SmoothTangents(curve) : curve;
    }

    //public static AnimationCurve FlattenTangents(AnimationCurve curve)
    //{
    //    Keyframe[] keyframes = curve.keys;

    //    for (int itr = 0; itr < keyframes.Length; ++itr)
    //    {
    //        keyframes[itr].inTangent = 0.0f;
    //        keyframes[itr].outTangent = 0.0f;
    //    }

    //    return curve;
    //}

    public static AnimationCurve SmoothTangents(AnimationCurve curve)
    {
        Keyframe[] keyframes = curve.keys;

        for (int itr = 0; itr < keyframes.Length; ++itr)
        {
            curve.SmoothTangents(itr, 0.0f);
        }

        return curve;
    }
}