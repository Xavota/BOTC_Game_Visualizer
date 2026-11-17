using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class DemonBluffsArea : MonoBehaviour
{
  public event TokensManager.DemonBluffChangedDelegate OnBluffChanged;
  public event TokensManager.VoidDelegate OnExpand;

  [SerializeField] private HBox tokensParent;
  [SerializeField] private List<BasicRoleToken> tokens;
  [SerializeField] private GameObject minimizeBtn;
  [SerializeField] private GameObject maximizeBtn;


  // Start is called once before the first execution of Update after the MonoBehaviour is created
  public void Init(List<string> info)
  {
    CreateTokens(info);
    Maximize();
  }

  public void CreateTokens(List<string> info)
  {
    if(tokensParent == null) return;
    tokensParent.enabled = true;
    tokensParent.OnFinishedUpdate -= OnRowUpdate;
    tokensParent.OnFinishedUpdate += OnRowUpdate;

    var tManager = TokensManager.Instance;

    for (int i = 0; i < info.Count; ++i)
    {
      BasicRoleToken token = null;
      if (i >= tokens.Count)
      {
        if(tManager.basicRoleTokenPrefab == null) break;

        token = Instantiate(
          tManager.basicRoleTokenPrefab.gameObject,
          tokensParent.transform
        ).GetComponent<BasicRoleToken>();
        tokens.Add(token);
      }

      token = tokens[i];

      if(token == null) continue;

      token.Init(info[i]);
      token.SetAbilityReminderSide(false);
      token.OnLeftClick += OnTokenClicked;
    }

    for (int i = info.Count; i < tokens.Count; ++i)
    {
      BasicRoleToken token = tokens[i];
      if(token == null) continue;
      token.Init(null);
      token.OnLeftClick += OnTokenClicked;
    }
  }

  public void RemoveRoles()
  {
    for(int i = 0; i < tokens.Count; ++i)
    {
      BasicRoleToken token = tokens[i];
      if(token == null) continue;
      token.Init("");
    }
  }

  private void OnRowUpdate(object o, System.EventArgs _)
  {
    tokensParent.enabled = false;
  }

  private void OnTokenClicked(object o, System.EventArgs args)
  {
    TokensManager.Instance.AskForRole(
      "Select a role for the demon bluffs:",
      (id) =>
      {
        BasicRoleToken roleToken = (BasicRoleToken)o;
        roleToken.ChangeRole(id);
        OnBluffChanged?.Invoke(tokens.FindIndex((bluff) => { return roleToken == bluff; }), id);
      },
      true, true
    );
  }

  public void Minimize()
  {
    if(tokensParent != null) tokensParent.gameObject.SetActive(false);
    if(minimizeBtn != null) minimizeBtn.gameObject.SetActive(false);
    if(maximizeBtn != null) maximizeBtn.gameObject.SetActive(true);
  }

  public void Maximize()
  {
    if(tokensParent != null) tokensParent.gameObject.SetActive(true);
    if(minimizeBtn != null) minimizeBtn.gameObject.SetActive(true);
    if(maximizeBtn != null) maximizeBtn.gameObject.SetActive(false);
  }

  public void Expand()
  {
    OnExpand?.Invoke();
  }
}