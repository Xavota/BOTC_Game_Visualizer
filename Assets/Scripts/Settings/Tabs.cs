using UnityEngine;
using System.Collections.Generic;

public class Tabs : MonoBehaviour
{
  public List<Tab> tabs = new();

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    for (int i = 0; i < tabs.Count; i++)
    {
      Tab tab = tabs[i];
      tab.HideTab();

      int tabIndex = i; // Capture the current index for the closure
      tab.tabChange += () => TabChanged(tabIndex);
    }
    tabs[0].ShowTab();
  }

  void TabChanged(int index)
  {
    foreach (Tab tab in tabs)
    {
      tab.HideTab();
    }
    tabs[index].ShowTab();
  }
}