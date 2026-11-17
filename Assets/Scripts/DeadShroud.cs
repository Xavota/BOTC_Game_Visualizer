using System;
using UnityEngine;
using UnityEngine.UI;

public class DeadShroud : MonoBehaviour
{
  public event EventHandler OnEnter;
  public event EventHandler OnExit;

  [SerializeField] private RawImage shadow;
  [SerializeField] private RawImage img;
  [SerializeField] private UIMouseDetector mouseDetector;
  public event EventHandler OnLeftClick;

  private bool dead = false;

  public void Init(bool dead)
  {
    SetState(dead);

    if(mouseDetector != null)
    {
      mouseDetector.OnEnter -= OnEntered;
      mouseDetector.OnExit -= OnExited;

      mouseDetector.OnEnter += OnEntered;
      mouseDetector.OnExit += OnExited;

      mouseDetector.OnLeftClick += (o, _) => OnLeftClick.Invoke(o, _);
    }
  }

  public void OnEntered(object o, System.EventArgs args)
  {
    if(shadow != null) shadow.color = new Color(1, 1, 1, dead ? 1.0f : 0.5f);
    if(img != null) img.color = new Color(1, 1, 1, dead ? 1.0f : 0.5f);
    OnEnter?.Invoke(this, args);
  }
  public void OnExited(object o, System.EventArgs args)
  {
    if(shadow != null) shadow.color = new Color(1, 1, 1, dead ? 1.0f : 0.01f);
    if(img != null) img.color = new Color(1, 1, 1, dead ? 1.0f : 0.01f);
    OnExit?.Invoke(this, args);
  }

  public void SetState(bool dead)
  {
    this.dead = dead;
    if(shadow != null) shadow.color = new Color(
      1, 1, 1,
      dead ?
      1.0f :
      ((mouseDetector?.MouseHovering ?? false) ? 0.5f : 0.01f)
    );
    if(img != null) img.color = new Color(
      1, 1, 1,
      dead ?
      1.0f :
      ((mouseDetector?.MouseHovering ?? false) ? 0.5f : 0.01f)
    );
  }
}