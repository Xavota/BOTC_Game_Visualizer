using System;
using System.Collections.Generic;
using UnityEngine;

public class FabledArea : MonoBehaviour
{
  public event TokensManager.BoolReturnDelegate OnTokenRemoved;

  [SerializeField] private HBox tokensParent;
  private List<BasicRoleToken> tokens = new();
  [SerializeField] private GameObject minimizeBtn;
  [SerializeField] private GameObject maximizeBtn;


  // Start is called once before the first execution of Update after the MonoBehaviour is created
  public void Init(List<RoleInfo> info)
  {
    if(tokensParent == null) return;
    CreateTokens(info);
    Maximize();
    tokensParent.OnFinishedUpdate -= OnRowUpdate;
    tokensParent.OnFinishedUpdate += OnRowUpdate;
  }

  private void OnRowUpdate(object o, System.EventArgs _)
  {
    tokensParent.enabled = false;
  }

  public void CreateTokens(List<RoleInfo> info)
  {
    ClearTokens();

    var tManager = TokensManager.Instance;

    if(tManager.fabledTokenPrefab == null) return;

    foreach (var i in info)
    {
      AddToken(i.id);
    }
  }

  public void ClearTokens()
  {
    foreach (var t in tokens)
    {
      Destroy(t.gameObject);
    }
    tokens.Clear();
  }

  public void AddToken(string id)
  {
    if(tokensParent == null) return;
    tokensParent.enabled = true;

    var tManager = TokensManager.Instance;

    if(tManager.fabledTokenPrefab == null) return;

    var token = Instantiate(
      tManager.fabledTokenPrefab.gameObject,
      tokensParent.transform
    ).GetComponent<BasicRoleToken>();

    if(token == null) return;

    token.Init(id, true, true);
    token.SetAbilityReminderSide(false);
    token.OnLeftClick += OnTokenLeftClick;
    tokens.Add(token);
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

  private void OnTokenLeftClick(object o, System.EventArgs e)
  {
    var tokenObj = (o as BasicRoleToken);
    if(tokenObj == null) return;
    if (OnTokenRemoved?.Invoke(tokenObj.role)?? false)
    {
      Destroy(tokenObj.gameObject);
      tokens.Remove(tokenObj);

      if(tokensParent == null) return;
      tokensParent.enabled = true;
    }
  }

  public void ReloadNightOrderInfo()
  {
    foreach (BasicRoleToken token in tokens)
    {
      token.ReloadNightOrderInfo();
    }
  }
}