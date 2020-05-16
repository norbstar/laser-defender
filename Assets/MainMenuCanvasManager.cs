using UnityEngine;

public class MainMenuCanvasManager : MonoBehaviour
{
    [SerializeField] MainMenuScrollbarManager mainMenuScrollbarManager;
    [SerializeField] AudioClip menuTransitionAudio, subMenuTransitionAudio, subMenuSelectionAudio;

    void Awake()
    {
        ResolveComponents();

        mainMenuScrollbarManager.RegisterDelegates(new MainMenuScrollbarManager.Delegates
        {
            OnItemTransitionDelegate = OnItemTransition,
            OnSubItemTransitionDelegate = OnSubItemTransition
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            AudioSource.PlayClipAtPoint(subMenuSelectionAudio, Camera.main.transform.position, 0.25f);

            var audioSourceModifier = Camera.main.GetComponent<AudioSourceModifier>() as AudioSourceModifier;
            audioSourceModifier.TransformVolume(0.0f, 1.0f);
                
            var abstractMainMenuItemManager = mainMenuScrollbarManager.GetSelectMainMenuItemManager();
            abstractMainMenuItemManager.ActionSelectedSubMenuItem();
        }
    }

    public void OnItemTransition()
    {
        AudioSource.PlayClipAtPoint(menuTransitionAudio, Camera.main.transform.position, 0.1f);
    }

    public void OnSubItemTransition()
    {
        AudioSource.PlayClipAtPoint(subMenuTransitionAudio, Camera.main.transform.position, 0.1f);
    }

    private void ResolveComponents() { }
}