using TMPro;
using UnityEngine;

public class NamePopup : MonoBehaviour
{
  public event TokensManager.NameDelegate OnNameChosen;
  public event TokensManager.VoidDelegate OnClosed;


  [SerializeField] private TMP_Text titleText;
  [SerializeField] private TMP_InputField nameField;


  public void Focus()
  {
    if (nameField != null) nameField.ActivateInputField();
  }

  public void SetTitle(string title)
  {
    if(titleText != null) titleText.text = title;
  }

  public void NameChosen()
  {
    if (nameField == null)
    {
      OnNameChosen?.Invoke("");
      return;
    }

    OnNameChosen?.Invoke(nameField.text);
    nameField.text = "";
  }
  public void Close()
  {
    OnClosed?.Invoke();
  }
}