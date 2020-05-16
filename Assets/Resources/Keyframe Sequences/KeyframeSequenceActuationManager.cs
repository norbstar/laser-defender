using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class KeyframeSequenceActuationManager : MonoBehaviour, IActuate
{
    public delegate void OnKeyframe(KeyframeConfig keyframeConfig);

    private OnKeyframe onKeyframeDelegate;

    [SerializeField] KeyframeSequenceConfig keyframeSequenceConfig;

    private GameObject keyframeSequencePrefab;
    private IList<Transform> keyframes;
    private IDictionary<float, IList<TransformKeyframeConfig>> collection;

    public class TransformKeyframeConfig
    {
        public Transform Transform { get; set; }
        public KeyframeConfig KeyframeConfig { get; set; }
    }

    void Awake()
    {
        ResolveComponents();

        collection = new SortedDictionary<float, IList<TransformKeyframeConfig>>();
    }

    private void ResolveComponents() { }

    public void Actuate(IConfiguration configuration = null)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(ActuateCoroutine());
        }
    }

    private void ResolveDependencies()
    {
        keyframeSequencePrefab = keyframeSequenceConfig.GetKeyframeSequencePrefab();
        keyframes = keyframeSequenceConfig.GetKeyframes();
    }

    public void RegisterDelegate(OnKeyframe onKeyframeDelegate)
    {
        this.onKeyframeDelegate = onKeyframeDelegate;
    }

    private IEnumerator ActuateCoroutine()
    {
        ResolveDependencies();

        for (int itr = 0; itr < keyframes.Count; ++itr)
        {
            var keyframeConfig = keyframes[itr].GetComponent<KeyframeConfig>() as KeyframeConfig;

            float keyframe = keyframeConfig.GetKeyframe();

            if (collection.ContainsKey(keyframe))
            {
                collection[keyframe].Add(new TransformKeyframeConfig
                {
                    Transform = keyframes[itr].transform,
                    KeyframeConfig = keyframeConfig
                });
            }
            else
            {
                IList<TransformKeyframeConfig> keyframeConfigs = new List<TransformKeyframeConfig>();
                keyframeConfigs.Add(new TransformKeyframeConfig
                {
                    Transform = keyframes[itr].transform,
                    KeyframeConfig = keyframeConfig
                });

                collection.Add(keyframe, keyframeConfigs);
            }
        }

        float startTime = Time.time;

        while (collection.Count > 0)
        {
            float now = Time.time - startTime;
            var thisKeyframe = collection.Select(d => d.Key).First();

            if (now >= thisKeyframe)
            {
                IList<TransformKeyframeConfig> configs = collection[thisKeyframe];

                foreach (TransformKeyframeConfig config in configs)
                {
                    Transform transform = config.Transform;
                    KeyframeConfig keyframeConfig = config.KeyframeConfig;

                    if (keyframeConfig.tag.Equals("Keyframe"))
                    {
                        // TODO
                    }
                    else if (keyframeConfig.tag.Equals("Prefab Keyframe"))
                    {
                        PrefabKeyframeConfig prefabKeyframeConfig = (PrefabKeyframeConfig) keyframeConfig;
                        GameObject prefab = prefabKeyframeConfig.GetPrefab();
                        var keyframePrefab = Instantiate(prefab, new Vector3(this.transform.position.x + transform.position.x, this.transform.position.y + transform.position.y, this.transform.position.z - 0.1f), transform.rotation, this.transform) as GameObject;
                        keyframePrefab.transform.localScale = transform.localScale;

                        AudioClip audioClip = prefabKeyframeConfig.GetAudioClip();

                        if (audioClip != null)
                        {
                            float volume = prefabKeyframeConfig.GetAudioVolume();

                            if (prefabKeyframeConfig.GetScaleAudioToPrefabScale())
                            {
                                volume *= transform.localScale.magnitude;
                            }

                            AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, prefabKeyframeConfig.GetAudioVolume());
                        }

                        if (prefabKeyframeConfig.GetDestroyPrefab())
                        {
                            float destroyAfterMs = prefabKeyframeConfig.GetDestroyPrefabAfterMs();
                            Destroy(keyframePrefab, destroyAfterMs);
                        }
                    }
                    else if (keyframeConfig.tag.Equals("Input Keyframe"))
                    {
                        //InputKeyframeConfig inputKeyframeConfig = (InputKeyframeConfig) keyframeConfig;
                        //Vector2 inputVector = inputKeyframeConfig.GetInput();

                        onKeyframeDelegate?.Invoke(keyframeConfig);
                    }
                }

                collection.Remove(thisKeyframe);
            }

            yield return null;
        }
    }
}