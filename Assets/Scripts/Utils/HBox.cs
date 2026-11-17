using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HBoxAlignment
{
  Left,
  Center,
  Right
}

[ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
public class HBox : MonoBehaviour
{
  public event System.EventHandler OnFinishedUpdate;

  private List<RectTransform> children = new();

  public HBoxAlignment alignment = HBoxAlignment.Left;
  public float spacing = 0;
  public float paddingLeft = 0;
  public float paddingRight = 0;
  public bool ignoreInactiveChildren = true;
  public bool adjustWidthToFitChildren = false;

  // Update is called once per frame
  void Update()
  {
    GetChildrenTransform();
    SetAllChildrenPivotAndAnchors();
    PlaceChildrenOnSequence();
    if (adjustWidthToFitChildren)
    {
      GetComponent<RectTransform>().sizeDelta = new Vector2(GetTotalWidth(), GetComponent<RectTransform>().sizeDelta.y);
    }
    OnFinishedUpdate?.Invoke(this, System.EventArgs.Empty);
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
        child.pivot = new Vector2(alignment == HBoxAlignment.Left ? 0 : alignment == HBoxAlignment.Right ? 1 : 0.5f, child.pivot.y);
        child.anchorMin = new Vector2(alignment == HBoxAlignment.Left ? 0 : alignment == HBoxAlignment.Right ? 1 : 0.5f, child.anchorMin.y);
        child.anchorMax = new Vector2(alignment == HBoxAlignment.Left ? 0 : alignment == HBoxAlignment.Right ? 1 : 0.5f, child.anchorMax.y);
      }
    }
  }

  public float GetTotalWidth()
  {
    float totalWidth = 0;
    foreach(var child in children)
    {
      if(!IsChildIgnored(child))
      {
        totalWidth += child.sizeDelta.x * child.localScale.x;
      }
    }
    int activeChildrenCount = children.Count(child => !IsChildIgnored(child));
    totalWidth += Mathf.Max(0, activeChildrenCount - 1) * spacing;
    return totalWidth + paddingLeft + paddingRight;
  }

  private void PlaceChildrenOnSequence()
  {
    if(alignment == HBoxAlignment.Left)
    {
      PlaceChildrenLeftToRight();
    }
    else if(alignment == HBoxAlignment.Center)
    {
      PlaceChildrenCenter();
    }
    else
    {
      PlaceChildrenRightToLeft();
    }
  }

  private void PlaceChildrenLeftToRight()
  {
    float currentX = paddingLeft;
    foreach(var child in children)
    {
      if(!IsChildIgnored(child))
      {
        child.anchoredPosition = new Vector2(currentX, child.anchoredPosition.y);
        currentX += child.sizeDelta.x * child.localScale.x + spacing;
      }
    }
  }

  private void PlaceChildrenCenter()
  {
    float totalWidth = GetTotalWidth();
    float currentX = -totalWidth / 2 + paddingLeft;
    foreach(var child in children)
    {
      if(!IsChildIgnored(child))
      {
        child.anchoredPosition = new Vector2(currentX + child.sizeDelta.x * child.localScale.x / 2, child.anchoredPosition.y);
        currentX += child.sizeDelta.x * child.localScale.x + spacing;
      }
    }
  }

  private void PlaceChildrenRightToLeft()
  {
    IEnumerable<RectTransform> enumerableThing = children;
    float currentX = -paddingRight;
    foreach(var child in enumerableThing.Reverse())
    {
      if(!IsChildIgnored(child))
      {
        child.anchoredPosition = new Vector2(currentX, child.anchoredPosition.y);
        currentX -= child.sizeDelta.x * child.localScale.x + spacing;
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