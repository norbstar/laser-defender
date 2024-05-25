using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(KeyframeSequenceActuationManager))]
public class IssueOrdersSequenceManager : MonoBehaviour, IActuate
{
    public delegate void OnStartAnimation();
    public delegate void OnEndAnimation();

    public class Delegates
    {
        public OnStartAnimation OnStartAnimationDelegate { get; set; }
        public OnEndAnimation OnEndAnimationDelegate { get; set; }
    }

    //[Header("Audio")]
    //[SerializeField] float syncAudioStartTime;

    private Delegates delegates;
    private GameObject buddy;
    private Animator animator;
    private AudioSource audioSource;
    private ScriptedShipController scriptedShipController;
    private KeyframeSequenceActuationManager keyframeSequenceActuationManager;
    //private float startTime;
    //private bool syncAudio;

    void Awake()
    {
        ResolveComponents();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if ((syncAudio) && ((Time.time - startTime) >= syncAudioStartTime))
    //    {
    //        syncAudio = false;
    //        StartCoroutine(SyncAudio());
    //    }
    //}

    private void ResolveComponents()
    {
        buddy = GameObject.FindGameObjectWithTag("Buddy") as GameObject;
        animator = GetComponent<Animator>() as Animator;
        audioSource = GetComponent<AudioSource>() as AudioSource;
        keyframeSequenceActuationManager = GetComponent<KeyframeSequenceActuationManager>() as KeyframeSequenceActuationManager;
        scriptedShipController = buddy.GetComponent<ScriptedShipController>() as ScriptedShipController;
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    public void Actuate(IConfiguration configuration = null)
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(Co_Actuate());
        }
    }

    private IEnumerator Co_Actuate()
    {
        ResolveDependencies();

        keyframeSequenceActuationManager.RegisterDelegate(OnKeyframe);
        keyframeSequenceActuationManager.Actuate();

        scriptedShipController.Actuate();

        //startTime = Time.time;
        //syncAudio = true;

        animator.SetBool("actuate", true);

        yield return null;
    }

    private void ResolveDependencies() { }

    //private IEnumerator SyncAudio()
    //{
    //    if (!audioSource.isPlaying)
    //    {
    //        audioSource.Play();

    //        yield return null;

    //        while (audioSource.isPlaying)
    //        {
    //            yield return null;
    //        }
    //    }
    //}

    public void OnKeyframe(KeyframeConfig keyframeConfig)
    {
        InputKeyframeConfig inputKeyframeConfig = (InputKeyframeConfig) keyframeConfig;
        Vector2 input = inputKeyframeConfig.GetInput();

        //Debug.Log($"OnKeyframe Input: {input}");

        scriptedShipController.EnqueueInput(input);
    }

    public void OnAnimationStart()
    {
        //Debug.Log($"OnAnimationStart");

        delegates?.OnStartAnimationDelegate?.Invoke();
    }

    public void OnAnimationEnterStart()
    {
        //Debug.Log($"OnAnimationEnterStart");
    }
    
    public void OnAnimationEnterEnd()
    {
        //Debug.Log($"OnAnimationEnterEnd");

        audioSource.Play();
    }

    public void OnAnimationExitStart()
    {
        //Debug.Log($"OnAnimationExitStart");
    }

    public void OnAnimationExitEnd()
    {
        //Debug.Log($"OnAnimationExitEnd");
    }

    public void OnAnimationEnd()
    {
        //Debug.Log($"OnAnimationEnd");

        delegates?.OnEndAnimationDelegate?.Invoke();
        Destroy(gameObject);
    }
}