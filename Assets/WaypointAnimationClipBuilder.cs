#if (UNITY_EDITOR) 

using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

public class WaypointAnimationClipBuilder : MonoBehaviour
{
   public static float ClipDuration = 1.0f;

   [SerializeField] GameObject[] paths;

   // Start is called before the first frame update
   void Start() => StartCoroutine(Co_CreateWaypointAnimationClips());

   private IEnumerator Co_CreateWaypointAnimationClips()
   {
       foreach (GameObject path in paths)
       {
           var pathConfig = path.GetComponent<PathConfig>();
           PathConfig.Smoothing smoothing = pathConfig.GetSmoothing();

           string rawAssetPath = AssetDatabase.GetAssetPath(path);
           int lastIndex = rawAssetPath.LastIndexOf('/');
           string assetPath = rawAssetPath.Substring(0, lastIndex);
           string assetName = rawAssetPath.Substring(lastIndex + 1);

           //Debug.Log($"Asset Game Object: {path.name} Raw Path: {rawAssetPath} Asset Name: {assetName} Extrapolated Path: {assetPath}");

           int keyframeCount = path.transform.childCount;

           AnimationCurve xAnimationCurve = CreateXCurve(path.transform, /*duration*/ClipDuration, keyframeCount, !smoothing.xTangents);
           AnimationCurve yAnimationCurve = CreateYCurve(path.transform, /*duration*/ClipDuration, keyframeCount, !smoothing.yTangents);

           AnimationClip animationClip = new AnimationClipBuilder()
               .AddCurve("localPosition.x", xAnimationCurve)
               .AddCurve("localPosition.y", yAnimationCurve)
               .CreateClip();

           UnityEditorAnimationFunctions.SaveClip(animationClip, assetPath, assetName);
       }

       yield return null;
   }

   private AnimationCurve CreateXCurve(Transform transform, float duration, int keyframeCount, bool flattenTangents)
   {
       var keyframes = new List<Keyframe>();
       float keyFrameDuration = duration / (keyframeCount);

       for (int itr = 0; itr < transform.childCount; ++itr)
       {
           float timestamp = (itr + 1) * keyFrameDuration;
           keyframes.Add(CreateKeyframe(timestamp, transform.GetChild(itr).position.x));
       }

       return AnimationFunctions.CreateCustomCurve(keyframes.ToArray(), flattenTangents);
   }

   private AnimationCurve CreateYCurve(Transform transform, float duration, int keyframeCount, bool flattenTangents)
   {
       var keyframes = new List<Keyframe>();
       float keyFrameDuration = duration / (keyframeCount);

       for (int itr = 0; itr < transform.childCount; ++itr)
       {
           float timestamp = (itr + 1) * keyFrameDuration;
           keyframes.Add(CreateKeyframe(timestamp, transform.GetChild(itr).position.y));
       }

       return AnimationFunctions.CreateCustomCurve(keyframes.ToArray(), flattenTangents);
   }

   private Keyframe CreateKeyframe(float timestamp, float value)
   {
       return new Keyframe(timestamp, value);
   }
}

#endif