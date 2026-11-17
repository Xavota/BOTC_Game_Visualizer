using UnityEngine;
using System.Collections.Generic;

public class Tab : MonoBehaviour
{
  public delegate void TabChange();
  public List<GameObject> tabObjs = new();
  public TabChange tabChange;

  public void SelectTab()
  {
    ShowTab();
    tabChange?.Invoke();
  }

  public void ShowTab()
  {
    foreach(GameObject obj in tabObjs)
    {
      obj.SetActive(true);
    }
  }

  public void HideTab()
  {
    foreach(GameObject obj in tabObjs)
    {
      obj.SetActive(false);
    }
  }

  public void PrintName()
  {
    Debug.Log($"Tab name: {gameObject.name}");
  }
}