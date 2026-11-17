using System.Collections.Generic;
using UnityEngine;

public class ShowDemonBluffs : MonoBehaviour
{
  public event TokensManager.VoidDelegate OnClose;

  [SerializeField] private HBox tokensParent;
  [SerializeField] private List<BasicRoleToken> tokens;


  // Start is called once before the first execution of Update after the MonoBehaviour is created
  public void Init(List<string> info)
  {
    CreateTokens(info);
  }

  public void CreateTokens(List<string> info)
  {
    if(tokensParent == null) return;
    tokensParent.enabled = true;
    tokensParent.OnFinishedUpdate -= OnRowUpdate;
    tokensParent.OnFinishedUpdate += OnRowUpdate;

    var tManager = TokensManager.Instance;

    for(int i = 0; i < info.Count; ++i)
    {
      BasicRoleToken token = null;
      if(i >= tokens.Count)
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
    }

    for(int i = info.Count; i < tokens.Count; ++i)
    {
      BasicRoleToken token = tokens[i];
      if(token == null) continue;
      token.Init(null);
    }
  }

  private void OnRowUpdate(object o, System.EventArgs _)
  {
    tokensParent.enabled = false;
  }

  public void Close()
  {
    OnClose?.Invoke();
  }
}