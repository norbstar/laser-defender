using UnityEngine;

public class MainMenuScoreboardManager : AbstractMainMenuItemManager
{
    public override bool IsSubMenuItemEnabled(int index)
    {
        SubMenuItem subMenuItem = GetSubMenuItem(index);
        return (gameObject.gameObject.tag.Equals("Active Menu Item"));
    }
}