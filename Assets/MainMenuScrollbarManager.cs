using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class MainMenuScrollbarManager : MonoBehaviour
{
    public delegate void OnItemTransition();
    public delegate void OnSubItemTransition();

    public class Delegates
    {
        public OnItemTransition OnItemTransitionDelegate { get; set; }
        public OnSubItemTransition OnSubItemTransitionDelegate { get; set; }
    }

    public enum FocusType
    {
        FOCUS,
        REVERT
    }

    [SerializeField] GameObject[] menuItems;

    private Delegates delegates;
    private Scrollbar scrollbar;
    private OnItemTransition onItemTransitionDelegate;
    private int menuItemCount;
    private int selectedMenuItem, selectedSubMenuItem;
    private Vector3 originalScale;
    private float scrollOffset;
    private bool enableControls;

    void Awake()
    {
        ResolveComponents();

        menuItemCount = menuItems.Length;
        selectedMenuItem = 0;
        originalScale = Vector3.one;
        scrollOffset = 1.0f / (menuItemCount - 1);
        enableControls = false;

        StartCoroutine(Co_HandleInterctions());
    }

    private void ResolveComponents()
    {
        scrollbar = GetComponent<Scrollbar>() as Scrollbar;
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    private IEnumerator Co_HandleInterctions()
    {
        GameObject gameObject = menuItems[selectedMenuItem];
        var abstractMainMenuItemManager = gameObject.GetComponent<AbstractMainMenuItemManager>() as AbstractMainMenuItemManager;
        abstractMainMenuItemManager.RegisterDelegates(new AbstractMainMenuItemManager.Delegates
        {
            OnSubMenuItemsShownDelegate = OnSubMenuItemsShown,
            OnSubMenuItemsHiddenDelegate = OnSubMenuItemsHidden
        });

        StartCoroutine(Co_TransformMenuItem(selectedMenuItem, FocusType.FOCUS));
        yield return StartCoroutine(abstractMainMenuItemManager.Co_SelectItem());

        while (true)
        {
            if (enableControls)
            {
                Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

                if (input.x != 0.0f)
                {
                    if ((input.x == -1.0f) && (selectedMenuItem - 1 >= 0))
                    {
                        gameObject = menuItems[selectedMenuItem];
                        abstractMainMenuItemManager = gameObject.GetComponent<AbstractMainMenuItemManager>() as AbstractMainMenuItemManager;
                        abstractMainMenuItemManager.RegisterDelegates(new AbstractMainMenuItemManager.Delegates
                        {
                            OnSubMenuItemsShownDelegate = OnSubMenuItemsShown,
                            OnSubMenuItemsHiddenDelegate = OnSubMenuItemsHidden
                        });
                        
                        StartCoroutine(abstractMainMenuItemManager.Co_DeselectItem());
                        StartCoroutine(Co_TransformMenuItem(selectedMenuItem, FocusType.REVERT));

                        yield return StartCoroutine(Co_ScrollToRelativePosition(-scrollOffset));

                        selectedMenuItem -= 1;
                        
                        gameObject = menuItems[selectedMenuItem];
                        abstractMainMenuItemManager = gameObject.GetComponent<AbstractMainMenuItemManager>() as AbstractMainMenuItemManager;
                        abstractMainMenuItemManager.RegisterDelegates(new AbstractMainMenuItemManager.Delegates
                        {
                            OnSubMenuItemsShownDelegate = OnSubMenuItemsShown,
                            OnSubMenuItemsHiddenDelegate = OnSubMenuItemsHidden
                        });

                        StartCoroutine(Co_TransformMenuItem(selectedMenuItem, FocusType.FOCUS));
                        StartCoroutine(abstractMainMenuItemManager.Co_SelectItem());

                        delegates?.OnItemTransitionDelegate?.Invoke();
                    }
                    else if ((input.x == 1.0f) && (selectedMenuItem + 1 <= (menuItemCount - 1)))
                    {
                        gameObject = menuItems[selectedMenuItem];
                        abstractMainMenuItemManager = gameObject.GetComponent<AbstractMainMenuItemManager>() as AbstractMainMenuItemManager;
                        abstractMainMenuItemManager.RegisterDelegates(new AbstractMainMenuItemManager.Delegates
                        {
                            OnSubMenuItemsShownDelegate = OnSubMenuItemsShown,
                            OnSubMenuItemsHiddenDelegate = OnSubMenuItemsHidden
                        });

                        StartCoroutine(abstractMainMenuItemManager.Co_DeselectItem());
                        StartCoroutine(Co_TransformMenuItem(selectedMenuItem, FocusType.REVERT));

                        yield return StartCoroutine(Co_ScrollToRelativePosition(scrollOffset));
                        selectedMenuItem += 1;
                        gameObject = menuItems[selectedMenuItem];
                        abstractMainMenuItemManager = gameObject.GetComponent<AbstractMainMenuItemManager>() as AbstractMainMenuItemManager;
                        abstractMainMenuItemManager.RegisterDelegates(new AbstractMainMenuItemManager.Delegates
                        {
                            OnSubMenuItemsShownDelegate = OnSubMenuItemsShown,
                            OnSubMenuItemsHiddenDelegate = OnSubMenuItemsHidden
                        });

                        StartCoroutine(Co_TransformMenuItem(selectedMenuItem, FocusType.FOCUS));
                        StartCoroutine(abstractMainMenuItemManager.Co_SelectItem());

                        delegates?.OnItemTransitionDelegate?.Invoke();
                    }
                }

                if (input.y != 0.0f)
                {
                    abstractMainMenuItemManager = gameObject.GetComponent<AbstractMainMenuItemManager>() as AbstractMainMenuItemManager;
                    AbstractMainMenuItemManager.SubMenuItem subMenuItem = abstractMainMenuItemManager.GetSelectedSubMenuItem();

                    if (subMenuItem != null)
                    {
                        try
                        {
                            int index = abstractMainMenuItemManager.GetIndexOfSubMenuItem(subMenuItem);

                            if (input.y == -1.0f)
                            {
                                AbstractMainMenuItemManager.SubMenuItem nextSubMenuItem = abstractMainMenuItemManager.GetNextSubMenuItemOfType(index, AbstractMainMenuItemManager.ItemType.ACTIVE);

                                if (nextSubMenuItem != null)
                                {
                                    index = abstractMainMenuItemManager.GetIndexOfSubMenuItem(nextSubMenuItem);
                                    abstractMainMenuItemManager.SelectSubMenuItem(index);

                                    delegates?.OnSubItemTransitionDelegate?.Invoke();
                                }
                            }
                            else if (input.y == 1.0f)
                            {
                                AbstractMainMenuItemManager.SubMenuItem previousSubMenuItem = abstractMainMenuItemManager.GetPreviousSubMenuItemOfType(index, AbstractMainMenuItemManager.ItemType.ACTIVE);

                                if (previousSubMenuItem != null)
                                {
                                    index = abstractMainMenuItemManager.GetIndexOfSubMenuItem(previousSubMenuItem);
                                    abstractMainMenuItemManager.SelectSubMenuItem(index);

                                    delegates?.OnSubItemTransitionDelegate?.Invoke();
                                }
                            }
                        }
                        catch { }

                        enableControls = false;
                        yield return new WaitForSeconds(0.1f);
                        enableControls = true;
                    }
                }
            }

            yield return null;
        }
    }

    private IEnumerator Co_ScrollToRelativePosition(float offset)
    {
        float originScrollBarValue = scrollbar.value;
        float targetScrollBarValue = scrollbar.value + offset;
        float startTime = Time.time;
        float duration = 0.1f;
        bool complete = false;

        enableControls = false;

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) / duration;

            var value = Mathf.Lerp(originScrollBarValue, targetScrollBarValue, fractionComplete);
            scrollbar.SetValueWithoutNotify(value);

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnScrollToRelativePositionComplete();
            }

            yield return null;
        }
    }

    private IEnumerator Co_TransformMenuItem(int index, FocusType focusType)
    {
        GameObject gameObject = menuItems[index];
        var abstractMainMenuItemManager = gameObject.GetComponent<AbstractMainMenuItemManager>() as AbstractMainMenuItemManager;
        gameObject = abstractMainMenuItemManager.GetMenuItem();

        Color originalColor = abstractMainMenuItemManager.GetOriginalMenuItemTextColor();

        float originScale = 1.0f;
        Color originColor = originalColor;
        float targetScale = 1.0f;
        Color targetColor = originalColor;
        float startTime = Time.time;
        float duration = 0.05f;
        bool complete = false;

        switch (focusType)
        {
            case FocusType.FOCUS:
                originScale = originalScale.x;
                originColor = originalColor;
                targetScale = originalScale.x * 1.15f;
                targetColor = Color.white;
                break;

            case FocusType.REVERT:
                originScale = originalScale.x * 1.15f;
                originColor = Color.white;
                targetScale = originalScale.x;
                targetColor = originalColor;
                break;
        }

        while (!complete)
        {
            var fractionComplete = (Time.time - startTime) / duration;

            var value = Mathf.Lerp(originScale, targetScale, fractionComplete);
            gameObject.transform.localScale = originalScale * value;
            abstractMainMenuItemManager.SetMenuItemColor(Color.Lerp(originColor, targetColor, fractionComplete));

            complete = fractionComplete >= 1f;

            if (complete)
            {
                OnTransformMenuItem();
            }

            yield return null;
        }
    }

    public GameObject GetSelectedMenuItem()
    {
        return menuItems[selectedMenuItem];
    }

    public int GetIndexOfMenuItem(GameObject menuItem)
    {
        for (int itr = 0; itr < menuItems.Length; ++itr)
        {
            if (menuItems[itr] == menuItem)
            {
                return itr;
            }
        }

        throw new IndexOutOfRangeException();
    }

    public AbstractMainMenuItemManager GetSelectMainMenuItemManager()
    {
        GameObject selectedMenyItem = GetSelectedMenuItem();
        return selectedMenyItem.GetComponent<AbstractMainMenuItemManager>() as AbstractMainMenuItemManager;
    }

    private void OnScrollToRelativePositionComplete() { }

    private void OnTransformMenuItem() { }

    private void OnSubMenuItemsShown(AbstractMainMenuItemManager abstractMainMenuItemManager)
    {
        AbstractMainMenuItemManager.SubMenuItem subMenuItem = abstractMainMenuItemManager.GetFirstSubMenuItemOfType(AbstractMainMenuItemManager.ItemType.ACTIVE);

        if (subMenuItem != null)
        {
            try
            {
                int index = abstractMainMenuItemManager.GetIndexOfSubMenuItem(subMenuItem);
                abstractMainMenuItemManager.SelectSubMenuItem(index);
            }
            catch { }
        }

        enableControls = true;
    }

    private void OnSubMenuItemsHidden(AbstractMainMenuItemManager abstractMainMenuItemManager)
    {
        AbstractMainMenuItemManager.SubMenuItem subMenuItem = abstractMainMenuItemManager.GetSelectedSubMenuItem();

        if (subMenuItem != null)
        {
            try
            {
                int index = abstractMainMenuItemManager.GetIndexOfSubMenuItem(subMenuItem);
                abstractMainMenuItemManager.RevertSubMenuItem(index);
            }
            catch { }
        }
    }
}