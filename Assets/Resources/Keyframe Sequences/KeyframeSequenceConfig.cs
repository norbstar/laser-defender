using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Keyframe Sequence Configuration")]
public class KeyframeSequenceConfig : ScriptableObject
{
    [SerializeField] GameObject keyframeSequencePrefab;

    public GameObject GetKeyframeSequencePrefab()
    {
        return keyframeSequencePrefab;
    }

    public IList<Transform> GetKeyframes()
    {
        var keyframes = new List<Transform>();

        foreach (Transform childTransform in keyframeSequencePrefab.transform)
        {
            keyframes.Add(childTransform);
        }

        return keyframes;
    }
}