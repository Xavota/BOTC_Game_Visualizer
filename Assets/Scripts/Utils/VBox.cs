using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum VBoxAlignment
{
  Top,
  Center,
  Bottom
}

[ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
public class VBox : MonoBehaviour
{
  private List<RectTransform> children = new();

  public VBoxAlignment alignment = VBoxAlignment.Top;
  public float spacing = 0;
  public float paddingTop = 0;
  public float paddingBottom = 0;
  public bool ignoreInactiveChildren = true;
  public bool adjustWidthToFitChildren = false;

  // Update is called once per frame
  void Update()
  {
    GetChildrenTransform();
    SetAllChildrenPivotAndAnchors();
    PlaceChildrenOnSequence();
    if(adjustWidthToFitChildren)
    {
      GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetTotalHeight());
    }
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

  private void SetAllChildrenPivotAndAnchors()
  {
    foreach(var child in children)
    {
      if(!IsChildIgnored(child))
      {
        child.pivot = new Vector2(child.pivot.x, alignment == VBoxAlignment.Top ? 1 : alignment == VBoxAlignment.Bottom ? 0 : 0.5f);
        child.anchorMin = new Vector2(child.anchorMin.x, alignment == VBoxAlignment.Top ? 1 : alignment == VBoxAlignment.Bottom ? 0 : 0.5f);
        child.anchorMax = new Vector2(child.anchorMax.x, alignment == VBoxAlignment.Top ? 1 : alignment == VBoxAlignment.Bottom ? 0 : 0.5f);
      }
    }
  }

  public float GetTotalHeight()
  {
    float totalHeight = 0;
    foreach(var child in children)
    {
      if(!IsChildIgnored(child))
      {
        totalHeight += child.sizeDelta.y * child.localScale.y;
      }
    }
    int activeChildrenCount = children.Count(child => !IsChildIgnored(child));
    totalHeight += Mathf.Max(0, activeChildrenCount - 1) * spacing;
    return totalHeight + paddingBottom + paddingTop;
  }

  private void PlaceChildrenOnSequence()
  {
    if(alignment == VBoxAlignment.Top)
    {
      PlaceChildrenTopToBottom();
    }
    else if(alignment == VBoxAlignment.Center)
    {
      PlaceChildrenCenter();
    }
    else
    {
      PlaceChildrenBottomToTop();
    }
  }

  private void PlaceChildrenTopToBottom()
  {
    float currentY = -paddingTop;
    foreach(var child in children)
    {
      if(!IsChildIgnored(child))
      {
        child.anchoredPosition = new Vector2(child.anchoredPosition.x, currentY);
        currentY -= child.sizeDelta.y * child.localScale.y + spacing;
      }
    }
  }

  private void PlaceChildrenCenter()
  {
    float totalHeight = GetTotalHeight();
    float currentY = totalHeight * 0.5f - paddingTop;
    foreach(var child in children)
    {
      if(!IsChildIgnored(child))
      {
        child.anchoredPosition = new Vector2(child.anchoredPosition.x, currentY - child.sizeDelta.y * child.localScale.y / 2);
        currentY -= child.sizeDelta.y * child.localScale.y + spacing;
      }
    }
  }

  private void PlaceChildrenBottomToTop()
  {
    IEnumerable<RectTransform> enumerableThing = children;
    float currentY = paddingBottom;
    foreach(var child in enumerableThing.Reverse())
    {
      if(!IsChildIgnored(child))
      {
        child.anchoredPosition = new Vector2(child.anchoredPosition.x, currentY);
        currentY += child.sizeDelta.y * child.localScale.y + spacing;
      }
    }
  }

  /*private void OnDrawGizmos()
  {
    Gizmos.color = Color.green;
    foreach(var child in children)
    {
      if(child != null)
      {
        Gizmos.DrawWireCube(child.position, child.sizeDelta);
      }
    }
  }*/
}