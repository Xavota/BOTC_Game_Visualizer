using TMPro;
using UnityEngine;

public class ShowPlayerReminder : MonoBehaviour
{
  public event TokensManager.VoidDelegate OnClose;

  [SerializeField] private TMP_Text titleText;
  [SerializeField] private BasicRoleToken basicRoleToken;
  [SerializeField] private Reminder remider;

  public void Init(string title, string role, ReminderInfo? rInfo)
  {
    if (titleText != null)
    {
      titleText.gameObject.SetActive(title != null);
      if (title != null) titleText.text = title;
    }
    if(basicRoleToken != null)
    {
      basicRoleToken.gameObject.SetActive(role != null);
      if(role != null) basicRoleToken.Init(role?? "");
    }
    if(remider != null)
    {
      remider.gameObject.SetActive(rInfo != null);
      if(rInfo != null) remider.Init(rInfo.Value);
    }
  }

  public void Close()
  {
    OnClose?.Invoke();
  }
}