using UnityEngine;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

public class PlayerReminderPopup : MonoBehaviour
{
  public event TokensManager.VoidDelegate OnClose;

  public event TokensManager.VoidDelegate OnShowYouAreRoleReminder;
  public event TokensManager.VoidDelegate OnShowSelectedYouReminder;
  public event TokensManager.VoidDelegate OnShowCharactersInPlayReminder;
  public event TokensManager.VoidDelegate OnShowCustomWithRoleReminder;
  public event TokensManager.VoidDelegate OnShowOnlyRoleReminder;

  public event TokensManager.VoidDelegate OnShowYouAreReminder;
  public event TokensManager.VoidDelegate OnShowCustomWithReminder;
  public event TokensManager.VoidDelegate OnShowOnlyReminder;

  public event TokensManager.VoidDelegate OnShowAttackedReminder;
  public event TokensManager.VoidDelegate OnShowSelectPlayerReminder;
  public event TokensManager.VoidDelegate OnShowSelectCharacterReminder;
  public event TokensManager.VoidDelegate OnShowCustomWithoutRoleReminder;

  public void Close()
  {
    OnClose?.Invoke();
  }

  public void ShowYouAreRoleReminder()
  {
    OnShowYouAreRoleReminder?.Invoke();
  }
  public void ShowSelectedYouReminder()
  {
    OnShowSelectedYouReminder?.Invoke();
  }
  public void ShowCharactersInPlayReminder()
  {
    OnShowCharactersInPlayReminder?.Invoke();
  }
  public void ShowCustomWithRoleReminder()
  {
    OnShowCustomWithRoleReminder?.Invoke();
  }
  public void ShowOnlyRoleReminder()
  {
    OnShowOnlyRoleReminder?.Invoke();
  }

  public void ShowYouAreReminder()
  {
    OnShowYouAreReminder?.Invoke();
  }
  public void ShowCustomWithReminder()
  {
    OnShowCustomWithReminder?.Invoke();
  }
  public void ShowOnlyReminder()
  {
    OnShowOnlyReminder?.Invoke();
  }

  public void ShowAttackedReminder()
  {
    OnShowAttackedReminder?.Invoke();
  }
  public void ShowSelectPlayerReminder()
  {
    OnShowSelectPlayerReminder?.Invoke();
  }
  public void ShowSelectCharacterReminder()
  {
    OnShowSelectCharacterReminder?.Invoke();
  }
  public void ShowCustomWithoutRoleReminder()
  {
    OnShowCustomWithoutRoleReminder?.Invoke();
  }
}