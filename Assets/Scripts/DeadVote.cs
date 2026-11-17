using UnityEngine;
using UnityEngine.UI;

public class DeadVote : MonoBehaviour
{
  //[SerializeField] private RawImage shadow;
  [SerializeField] private RawImage img;
  [SerializeField] private UIMouseDetector mouseDetector;
  public event System.EventHandler OnLeftClick;

  private bool voteless = false;

  public void Init(bool voteless)
  {
    SetState(voteless);

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
    //if(shadow != null) shadow.color = new Color(1, 1, 1, voteless ? 0.5f : 1.0f);
    if(img != null) img.color = new Color(1, 1, 1, voteless ? 0.5f : 1.0f);
  }
  public void OnExited(object o, System.EventArgs args)
  {
    //if(shadow != null) shadow.color = new Color(1, 1, 1, voteless ? 0.01f : 1.0f);
    if(img != null) img.color = new Color(1, 1, 1, voteless ? 0.01f : 1.0f);
  }

  public void SetState(bool voteless)
  {
    this.voteless = voteless;
    //if(shadow != null) shadow.color = new Color(
    //  1, 1, 1,
    //  voteless ?
    //  ((mouseDetector?.MouseHovering?? false) ? 0.5f : 0.01f) :
    //  1.0f
    //);
    if(img != null) img.color = new Color(
      1, 1, 1,
      voteless ?
      ((mouseDetector?.MouseHovering?? false) ? 0.5f : 0.01f) :
      1.0f
    );
  }
}