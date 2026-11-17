using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
public class AdjustToChildren : MonoBehaviour
{
  private List<RectTransform> children = new();

  public bool adjustWidth = false;
  public bool adjustHeight = false;
  public bool ignoreInactiveChildren = true;
  public Vector2 padding = new();
  public Vector2 minSize = new();
  public Vector2 maxSize = new();

  // Update is called once per frame
  void Update()
  {
    GetChildrenTransform();
    AdjustSize();
  }

  private void GetChildrenTransform()
  {
    children.Clear();
    foreach(Transform child in transform)
    {
      children.Add(child.GetComponent<RectTransform>());
    }
  }

  private bool IsChildIgnored(RectTransform child)
  {
    return child == null || (ignoreInactiveChildren && !child.gameObject.activeSelf);
  }

  private void AdjustSize()
  {
    float width = 0;
    float height = 0;
    foreach(var child in children)
    {
      if(!IsChildIgnored(child))
      {
        width = Mathf.Max(width, child.sizeDelta.x * child.localScale.x);
        height = Mathf.Max(height, child.sizeDelta.y * child.localScale.y);
      }
    }
    if(adjustWidth)
    {
      GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(width + padding.x * 2, minSize.x, maxSize.x <= 0.0f ? Mathf.Infinity : maxSize.x), GetComponent<RectTransform>().sizeDelta.y);
    }
    if(adjustHeight)
    {
      GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, Mathf.Clamp(height + padding.y * 2, minSize.y, maxSize.y <= 0.0f ? Mathf.Infinity : maxSize.y));
    }
  }
}