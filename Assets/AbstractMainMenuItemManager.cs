using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractMainMenuItemManager : MonoBehaviour
{
    public delegate void OnSubMenuItemsShown(AbstractMainMenuItemManager abstractMainMenuItemManager);
    public delegate void OnSubMenuItemsHidden(AbstractMainMenuItemManager abstractMainMenuItemManager);

    public class Delegates
    {
        public OnSubMenuItemsShown OnSubMenuItemsShownDelegate { get; set; }
        public OnSubMenuItemsHidden OnSubMenuItemsHiddenDelegate { get; set; }
    }

    private Delegates delegates;
    private SubMenuItem selectedSubMenuItem;

    public enum ItemType
    {
        ACTIVE,
        PASSIVE
    }

    public enum FocusType
    {
        FOCUS,
        REVERT
    }

    [Serializable]
    public class SubMenuItem
    {
        public GameObject gameObject;
        public string description;
    }

    [SerializeField] GameObject menuItem;
    [SerializeField] SubMenuItem[] subMenuItems;
    [SerializeField] GameObject description;

    private Text menuItemText;
    private Color originalMenuItemTextColor, originalSubMenuItemTextColor;

    void Awake()
    {
        ResolveComponents();

        originalMenuItemTextColor = menuItemText.color;
        ColorUtility.TryParseHtmlString("#C8C8C8", out originalSubMenuItemTextColor);
    }

    private void ResolveComponents()
    {
        menuItemText = menuItem.GetComponent<Text>() as Text;
    }

    public void RegisterDelegates(Delegates delegates)
    {
        this.delegates = delegates;
    }

    public abstract bool IsSubMenuItemEnabled(int index);

    public IEnumerator SelectItemCoroutine()
    {
        SubMenuItem[] subMenuItems = GetSubMenuItems();

        for (int itr = 0; itr < subMenuItems.Length; ++itr)
        {
            yield return StartCoroutine(TransformSubMenuItem(itr, AbstractMainMenuItemManager.FocusType.FOCUS));
        }

        delegates?.OnSubMenuItemsShownDelegate?.Invoke(this);
    }

    public IEnumerator DeselectItemCoroutine()
    {
        description.GetComponent<Text>().text = "";

        SubMenuItem[] subMenuItems = GetSubMenuItems();

        for (int itr = subMenuItems.Length - 1; itr >= 0; --itr)
        {
            yield return StartCoroutine(TransformSubMenuItem(itr, AbstractMainMenuItemManager.FocusType.REVERT));
        }

        delegates?.OnSubMenuItemsHiddenDelegate?.Invoke(this);
    }

    public GameObject GetMenuItem()
    {
        return menuItem;
    }
    
    public Color GetOriginalMenuItemTextColor()
    {
        return originalMenuItemTextColor;
    }

    public SubMenuItem GetSubMenuItem(int index)
    {
        return subMenuItems[index];
    }

    public int GetIndexOfSubMenuItem(SubMenuItem subMenuItem)
    {
        for (int itr = 0; itr < subMenuItems.Length; ++itr)
        {
            if (subMenuItems[itr].gameObject == subMenuItem.gameObject)
            {
                return itr;
            }
        }

        throw new IndexOutOfRangeException();
    }

    public SubMenuItem GetSubMenuItemOfType(int index, ItemType itemType)
    {
        SubMenuItem subMenuItem;

        try
        {
            subMenuItem = subMenuItems[index];

            switch (itemType)
            {
                case ItemType.ACTIVE:
                    if (subMenuItem.gameObject.tag.Equals("Active Menu Item"))
                    {
                        return subMenuItem;
                    }
                    break;

                case ItemType.PASSIVE:
                    if (subMenuItem.gameObject.tag.Equals("Passive Menu Item"))
                    {
                        return subMenuItem;
                    }
                    break;
            }
        }
        catch { }
            
        return null;
    }

    public SubMenuItem GetSelectedSubMenuItem()
    {
        return selectedSubMenuItem;
    }

    public SubMenuItem GetFirstSubMenuItemOfType(ItemType itemType)
    {
        for (int itr = 0; itr < subMenuItems.Length; ++itr)
        {
            SubMenuItem subMenuItem = GetSubMenuItemOfType(itr, itemType);

            if (subMenuItem != null)
            {
                return subMenuItem;
            }
        }

        return null;
    }

    public SubMenuItem GetPreviousSubMenuItemOfType(int index, ItemType itemType)
    {
        if (index - 1 >= 0)
        {
            for (int itr = index - 1; itr >= 0; --itr)
            {
                SubMenuItem subMenuItem = GetSubMenuItemOfType(itr, itemType);

                if (subMenuItem != null)
                {
                    return subMenuItem;
                }
            }
        }

        return null;
    }

    public SubMenuItem GetNextSubMenuItemOfType(int index, ItemType itemType)
    {
        if (index + 1 < subMenuItems.Length)
        {
            for (int itr = index + 1; itr < subMenuItems.Length; ++itr)
            {
                SubMenuItem subMenuItem = GetSubMenuItemOfType(itr, itemType);

                if (subMenuItem != null)
                {
                    return subMenuItem;
                }
            }
        }

        return null;
    }

    public SubMenuItem[] GetSubMenuItems()
    {
        return subMenuItems;
    }

    public void ActionSelectedSubMenuItem()
    {
        SubMenuItem subMenuItem = GetSelectedSubMenuItem();
        var actuation = subMenuItem.gameObject.GetComponent<IActuate>() as IActuate;
        actuation?.Actuate(new SubMenuItemConfiguration
        {
            SubMenuItem = subMenuItem.gameObject
        });
    }

    public void SetMenuItemColor(Color color)
    {
        menuItemText.color = color;
    }

    public void SelectSubMenuItem(int index)
    {
        if (selectedSubMenuItem != null)
        {
            int selectedIndex = GetIndexOfSubMenuItem(selectedSubMenuItem);
            DeselectSubMenuItem(selectedIndex);
        }

        SubMenuItem subMenuItem = GetSubMenuItem(index);
        var text = subMenuItem.gameObject.GetComponent<Text>() as Text;
        text.color = Color.white;

        description.GetComponent<Text>().text = subMenuItem.description;

        selectedSubMenuItem = subMenuItem;
    }

    public void RevertSubMenuItem(int index)
    {
        SubMenuItem subMenuItem = GetSubMenuItem(index);
        var text = subMenuItem.gameObject.GetComponent<Text>() as Text;
        text.color = new Color(originalSubMenuItemTextColor.r, originalSubMenuItemTextColor.g, originalSubMenuItemTextColor.b, 0.0f);

        selectedSubMenuItem = null;
    }

    public void DeselectSubMenuItem(int index)
    {
        SubMenuItem subMenuItem = GetSubMenuItem(index);
        var text = subMenuItem.gameObject.GetComponent<Text>() as Text;
        text.color = originalSubMenuItemTextColor;

        selectedSubMenuItem = null;
    }

    protected IEnumerator TransformSubMenuItem(int subMenuItem, FocusType focusType)
    {
        var text = subMenuItems[subMenuItem].gameObject.GetComponent<Text>() as Text;

        float originColor = 0.0f;
        float targetColor = 0.0f;
        float startTime = Time.time;
        float duration = 0.05f;
        bool complete = false;

        switch (focusType)
        {
            case FocusType.FOCUS:
                originColor = 0.0f;
                targetColor = 1.0f;
                break;

            case FocusType.REVERT:
                originColor = 1.0f;
                targetColor = 0.0f;
                break;
        }

        while (!complete)
        {
            float fractionComplete = (Time.time - startTime) / duration;

            float value = Mathf.Lerp(originColor, targetColor, fractionComplete);
            text.color = new Color(originalSubMenuItemTextColor.r, originalSubMenuItemTextColor.g, originalSubMenuItemTextColor.b, value);

            complete = (fractionComplete >= 1.0f);

            if (complete)
            {
                OnTransformSubMenuItem();
            }

            yield return null;
        }
    }

    private void OnTransformSubMenuItem() { }
}