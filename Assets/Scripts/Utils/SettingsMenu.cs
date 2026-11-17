using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
  // GRIMOIRE DELEGATES
  public TokensManager.VoidDelegate OnResetGame;
  public TokensManager.BoolDelegate OnTokensVisibilityChange;
  public TokensManager.BoolDelegate OnNightTimeChange;
  public TokensManager.BoolDelegate OnNightOrderVisibilityChange;

  // PLAYERS DELEGATES
  public TokensManager.VoidDelegate OnPlayerAdded;
  public TokensManager.VoidDelegate OnRemoveAllPlayers;
  public TokensManager.VoidDelegate OnShowReminder;

  // CHARACTERS DELEGATES
  public TokensManager.VoidDelegate OnSelectEdition;
  public TokensManager.VoidDelegate OnFabledAdded;
  public TokensManager.VoidDelegate OnRemoveAllCharacters;

  // HELP DELEGATES
  public TokensManager.VoidDelegate OnShowReferenceSheet;

  private float targetRotation = 0.0f;
  private float timeToRotate = 0.0f;


  [Header("TokenVisibility")]
  [SerializeField] private TMP_Text tokenVisibilityText;
  [SerializeField] private RawImage tokenVisibilityIcon;
  [SerializeField] private Texture2D tokenVisibilityHideImg;
  [SerializeField] private Texture2D tokenVisibilityShowImg;
  private bool tokensVisible = true;

  [Header("NightTime")]
  [SerializeField] private TMP_Text nightTimeText;
  [SerializeField] private RawImage nightTimeIcon;
  [SerializeField] private Texture2D nightTimeHideImg;
  [SerializeField] private Texture2D nightTimeShowImg;
  private bool nightTime = false;

  [Header("NightOrder")]
  [SerializeField] private TMP_Text nightOrderText;
  private bool nightOrderVisible = true;

  //
  // MENU ACTIONS
  //

  public void ToggleMenu()
  {
    if(targetRotation == 0.0f)
    {
      HideMenu();
    }
    else
    {
      ShowMenu();
    }
  }

  public void ShowMenu()
  {
    targetRotation = 0.0f;
    timeToRotate = 0.2f;
  }

  public void HideMenu()
  {
    targetRotation = 90.0f;
    timeToRotate = 0.2f;
  }


  //
  // GRIMOIRE
  //

  public void ResetGame()
  {
    OnResetGame?.Invoke();
  }

  public void ToggleTokensVisibility()
  {
    tokensVisible = !tokensVisible;
    OnTokensVisibilityChange?.Invoke(tokensVisible);

    if(tokenVisibilityText != null) tokenVisibilityText.text = tokensVisible ?
        "Hide Tokens" : "Show Tokens";
    if(tokenVisibilityIcon != null) tokenVisibilityIcon.texture = tokensVisible ?
        tokenVisibilityHideImg : tokenVisibilityShowImg;
  }

  public void ToggleNightTime()
  {
    nightTime = !nightTime;
    OnNightTimeChange?.Invoke(nightTime);

    if(nightTimeText != null) nightTimeText.text = nightTime ?
        "Day Time" : "Night Time";
    if(nightTimeIcon != null) nightTimeIcon.texture = nightTime ?
        nightTimeHideImg : nightTimeShowImg;
  }

  public void ToggleNightOrderVisibility()
  {
    nightOrderVisible = !nightOrderVisible;
    OnNightOrderVisibilityChange?.Invoke(nightOrderVisible);

    if(nightOrderText != null) nightOrderText.text = nightOrderVisible ?
        "Hide Night Order" : "Show Night Order";
  }

  //
  // PLAYERS
  //

  public void AddPlayer()
  {
    OnPlayerAdded?.Invoke();
  }

  public void RemoveAllPlayers()
  {
    OnRemoveAllPlayers?.Invoke();
  }

  public void ShowReminder()
  {
    OnShowReminder?.Invoke();
  }

  //
  // CHARACTERS
  //

  public void SelectEdition()
  {
    OnSelectEdition?.Invoke();
  }

  public void AddFabled()
  {
    OnFabledAdded?.Invoke();
  }

  public void RemoveAllCharacters()
  {
    OnRemoveAllCharacters?.Invoke();
  }

  //
  // HELP
  //

  public void ShowReferenceSheet()
  {
    OnShowReferenceSheet?.Invoke();
  }


  private void Update()
  {
    if (timeToRotate > 0.0f)
    {
      float rotationStep = (targetRotation - transform.eulerAngles.z) * Time.deltaTime / timeToRotate;
      transform.Rotate(0, 0, rotationStep);
      timeToRotate -= Time.deltaTime;
      if (timeToRotate <= 0.0f)
      {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetRotation);
      }
    }
  }
}