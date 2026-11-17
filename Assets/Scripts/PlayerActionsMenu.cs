using UnityEngine;

public class PlayerActionsMenu : MonoBehaviour
{
  public delegate void VoidDelegate();

  public event System.EventHandler OnEnter;
  public event System.EventHandler OnExit;
  public event VoidDelegate OnRename;
  public event VoidDelegate OnMove;
  public event VoidDelegate OnSwap;
  public event VoidDelegate OnRemove;
  public event VoidDelegate OnNominate;

  [SerializeField] private UIMouseDetector mouseDetector;

  public void Init()
  {
    mouseDetector.OnEnter += (o, _) => {
      OnEnter?.Invoke(this, System.EventArgs.Empty);
    };
    mouseDetector.OnExit += (o, _) =>
    {
      OnExit?.Invoke(this, System.EventArgs.Empty);
    };
  }

  public void Rename()
  {
    OnRename?.Invoke();
  }
  public void Move()
  {
    OnMove?.Invoke();
  }
  public void Swap()
  {
    OnSwap?.Invoke();
  }
  public void Remove()
  {
    OnRemove?.Invoke();
  }
  public void Nominate()
  {
    OnNominate?.Invoke();
  }
}