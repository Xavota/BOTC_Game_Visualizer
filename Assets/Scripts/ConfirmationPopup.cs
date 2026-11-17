using TMPro;
using UnityEngine;

public class ConfirmationPopup : MonoBehaviour
{
  public event TokensManager.VoidDelegate OnAccept;
  public event TokensManager.VoidDelegate OnClosed;

  [SerializeField] private TMP_Text title;


  public void Init(string title)
  {
    if (this.title == null) return;
    if (title == null) title = "";
    this.title.text = title;
  }


  public void Accept()
  {
    OnAccept?.Invoke();
  }
  public void Close()
  {
    OnClosed?.Invoke();
  }
}