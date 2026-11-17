using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum UIMouseDetectorState
{
  NotHovered,
  Hovered,
  WaitingForClick,
  WaitingNoDoubleClick,
  WaitingForDoubleClick,
  WaitingForLongClick
}

public class UIMouseDetector : MonoBehaviour,
  IPointerEnterHandler, IPointerExitHandler
{
  private InputAction leftClickAction;
  private InputAction rightClickAction;
  private InputAction middleClickAction;
  private void Awake()
  {
    leftClickAction = InputSystem.actions.FindAction("Click");
    rightClickAction = InputSystem.actions.FindAction("RightClick");
    middleClickAction = InputSystem.actions.FindAction("MiddleClick");
  }

  public bool MouseHovering
  {
    get { return state != UIMouseDetectorState.NotHovered; }
  }

  private UIMouseDetectorState state = UIMouseDetectorState.NotHovered;

  private bool mouseEntered = false;
  private bool mouseExited = false;
  private bool doubleClickTimeExpired = false;
  private bool longClickTimeExpired = false;

  private float timeToDoubleClick = 0.0f;
  private float doubleClickThreshold = 0.4f;
  private float timeToLongClick = 0.0f;
  private float longClickThreshold = 0.5f;

  private void ResetClocks()
  {
    timeToDoubleClick = 0.0f;
    timeToLongClick = 0.0f;
  }

  private void StateMachineState()
  {
    switch(state)
    {
      case UIMouseDetectorState.NotHovered:
        NotHovered();
        break;
      case UIMouseDetectorState.Hovered:
        Hovered();
        break;
      case UIMouseDetectorState.WaitingForClick:
        WaitingState();
        break;
      case UIMouseDetectorState.WaitingNoDoubleClick:
        WaitingNoDoubleClickState();
        break;
      case UIMouseDetectorState.WaitingForDoubleClick:
        WaitingForDoubleClickState();
        break;
    }
  }

  public void ResetStateMachine()
  {
    state = UIMouseDetectorState.NotHovered;
    ResetClocks();
    mouseEntered = false;
    mouseExited = false;
    doubleClickTimeExpired = false;
    longClickTimeExpired = false;
  }

  private void NotHovered()
  {
    if(leftClickAction.WasPressedThisFrame())
    {
      OnPointerClickOutside();
    }

    if (mouseEntered)
    {
      mouseEntered = false;
      state = UIMouseDetectorState.Hovered;
    }
  }
  private void Hovered()
  {
    OnPointerHover();

    if(mouseExited)
    {
      mouseExited = false;
      state = UIMouseDetectorState.NotHovered;
    }
    else if(leftClickAction.WasPressedThisFrame())
    {
      state = UIMouseDetectorState.WaitingForClick;
      timeToLongClick = longClickThreshold;
      //SetClocks();
    }
    else if(rightClickAction.WasPressedThisFrame())
    {
      OnPointerRightClick();
    }
    else if(middleClickAction.WasPressedThisFrame())
    {
      OnPointerMiddleClick();
    }
  }
  private void WaitingState()
  {
    if(mouseExited)
    {
      mouseExited = false;
      state = UIMouseDetectorState.NotHovered;
      ResetClocks();
    }
    else if(rightClickAction.WasPressedThisFrame())
    {
      state = UIMouseDetectorState.Hovered;
      OnPointerRightClick();
      ResetClocks();
    }
    else if(middleClickAction.WasPressedThisFrame())
    {
      state = UIMouseDetectorState.Hovered;
      OnPointerMiddleClick();
      ResetClocks();
    }
    else if(leftClickAction.WasReleasedThisFrame())
    {
      state = UIMouseDetectorState.WaitingForDoubleClick;
      OnPointerLeftClick();
      timeToLongClick = 0.0f;
      timeToDoubleClick = doubleClickThreshold;
    }
    else if(longClickTimeExpired)
    {
      longClickTimeExpired = false;
      OnPointerLongClick();
    }
  }
  private void WaitingNoDoubleClickState()
  {
    if(mouseExited)
    {
      mouseExited = false;
      state = UIMouseDetectorState.NotHovered;
      ResetClocks();
    }
    else if(rightClickAction.WasPressedThisFrame())
    {
      state = UIMouseDetectorState.Hovered;
      OnPointerRightClick();
      ResetClocks();
    }
    else if(middleClickAction.WasPressedThisFrame())
    {
      state = UIMouseDetectorState.Hovered;
      OnPointerMiddleClick();
      ResetClocks();
    }
    else if(leftClickAction.WasReleasedThisFrame())
    {
      state = UIMouseDetectorState.Hovered;
      OnPointerLeftClick();
      timeToLongClick = 0.0f;
    }
    else if(longClickTimeExpired)
    {
      longClickTimeExpired = false;
      OnPointerLongClick();
    }
  }
  private void WaitingForDoubleClickState()
  {
    if(mouseExited)
    {
      mouseExited = false;
      state = UIMouseDetectorState.NotHovered;
      ResetClocks();
    }
    else if(leftClickAction.WasPressedThisFrame())
    {
      state = UIMouseDetectorState.WaitingNoDoubleClick;
      OnPointerDoubleClick();
      timeToDoubleClick = 0.0f;
      timeToLongClick = longClickThreshold;
    }
    else if(doubleClickTimeExpired)
    {
      doubleClickTimeExpired = false;
      state = UIMouseDetectorState.Hovered;
    }
  }

  public event EventHandler OnEnter;
  public event EventHandler OnExit;
  public event EventHandler OnHover;
  public event EventHandler OnLeftClick;
  public event EventHandler OnRightClick;
  public event EventHandler OnMiddleClick;
  public event EventHandler OnDoubleClick;
  public event EventHandler OnLongClick;
  public event EventHandler OnClickOutside;

  public void OnPointerEnter(PointerEventData eventData)
  {
    mouseEntered = true;
    OnEnter?.Invoke(this, EventArgs.Empty);
  }
  public void OnPointerExit(PointerEventData eventData)
  {
    mouseExited = true;
    OnExit?.Invoke(this, EventArgs.Empty);
  }
  private void OnPointerHover()
  {
    OnHover?.Invoke(this, EventArgs.Empty);
  }

  private void OnPointerLeftClick()
  {
    OnLeftClick?.Invoke(this, EventArgs.Empty);
  }
  private void OnPointerDoubleClick()
  {
    OnDoubleClick?.Invoke(this, EventArgs.Empty);
  }
  private void OnPointerLongClick()
  {
    OnLongClick?.Invoke(this, EventArgs.Empty);
  }

  private void OnPointerRightClick()
  {
    OnRightClick?.Invoke(this, EventArgs.Empty);
  }
  private void OnPointerMiddleClick()
  {
    OnMiddleClick?.Invoke(this, EventArgs.Empty);
  }

  private void OnPointerClickOutside()
  {
    OnClickOutside?.Invoke(this, EventArgs.Empty);
  }

  private void Update()
  {
    if (timeToDoubleClick > 0.0f)
    {
      timeToDoubleClick -= Time.deltaTime;
      if (timeToDoubleClick <= 0.0f)
      {
        timeToDoubleClick = 0.0f;
        doubleClickTimeExpired = true;
      }
    }
    if(timeToLongClick > 0.0f)
    {
      timeToLongClick -= Time.deltaTime;
      if(timeToLongClick <= 0.0f)
      {
        timeToLongClick = 0.0f;
        longClickTimeExpired = true;
      }
    }

    StateMachineState();
  }
}