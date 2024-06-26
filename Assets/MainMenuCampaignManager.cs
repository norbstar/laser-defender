﻿using UnityEngine;

public class MainMenuCampaignManager : AbstractMainMenuItemManager
{
    public override bool IsSubMenuItemEnabled(int index)
    {
        SubMenuItem subMenuItem = GetSubMenuItem(index);
        return (subMenuItem.gameObject.tag.Equals("Active Menu Item"));
    }
}