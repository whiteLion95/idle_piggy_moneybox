using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private List<TabButton> tabButtons = new List<TabButton>();
    [SerializeField] private Sprite tabIdle;
    [SerializeField] private Sprite tabActive;
    [SerializeField] private TabButton selectedTab;
    [SerializeField] private List<GameObject> objectsToSwap;
    [SerializeField] private bool simpleTabs = false;

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            //button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        if (selectedTab && !simpleTabs) selectedTab.transform.SetAsFirstSibling();
        selectedTab.DisableSelectionOutline();

        selectedTab = button;
        ResetTabs();
        if (tabActive)
            button.background.sprite = tabActive;
        int index = tabButtons.IndexOf(button);
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
                objectsToSwap[i].SetActive(true);
            else
                objectsToSwap[i].SetActive(false);
        }

        if (!simpleTabs)
            button.transform.SetAsLastSibling();
    }

    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab)
            {
                continue;
            }

            if (!tabIdle) continue;
            button.background.sprite = tabIdle; 
        }
    }
}