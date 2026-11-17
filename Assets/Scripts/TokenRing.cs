using UnityEngine;
using System.Collections.Generic;

public class TokenRing : MonoBehaviour
{
  public delegate void PlayerActionDelegate(int playerIndex);
  public delegate void StatusChangeDelegate(int playerIndex, bool status);
  public delegate void RoleChangeDelegate(int playerIndex, string role);
  public delegate bool ReminderBoolReturnDelegate(int playerIndex, int reminderIndex);

  public event StatusChangeDelegate OnDeadStatusChange;
  public event StatusChangeDelegate OnVotelessStatusChange;
  public event RoleChangeDelegate OnRoleChange;
  public event PlayerActionDelegate OnReminderAdd;
  public event ReminderBoolReturnDelegate OnReminderRemove;

  public event PlayerActionDelegate OnRename;
  public event PlayerActionDelegate OnMove;
  public event PlayerActionDelegate OnSwap;
  public event PlayerActionDelegate OnRemove;
  public event PlayerActionDelegate OnStartNomination;
  public event PlayerActionDelegate OnNominate;

  [SerializeField] private float tokenRadius = 440.0f;

  [SerializeField] private List<PlayerToken> tokenObjs = new();

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  public void Init(List<PlayerInfo> tokenInfos)
  {
    CreateTokens(tokenInfos);
  }

  private void CreateTokens(List<PlayerInfo> tokenInfos)
  {
    var tManager = TokensManager.Instance;

    PlayerToken tokenPrefab = tManager.playerTokenPrefab;
    if(tokenPrefab == null)
    {
      tokenPrefab = Resources.Load<PlayerToken>("PlayerToken");
      if(tokenPrefab == null) return;
    }

    tokenObjs.Clear();

    int tokenCount = tokenInfos.Count;
    for(int i = 0; i < tokenCount; i++)
    {
      float tokenAngle = 360.0f / tokenCount * i - 90.0f;
      tokenAngle *= -Mathf.Deg2Rad;

      CreatePlayerToken(tokenPrefab, tokenInfos[i], tokenAngle);
    }
  }

  private void CreatePlayerToken(PlayerToken tokenPrefab, PlayerInfo info, float angle)
  {
    Vector3 tokenPosition =
      transform.position +
      new Vector3(
        Mathf.Cos(angle),
        Mathf.Sin(angle),
        0.0f
      ) *
      tokenRadius;

    PlayerToken token = Instantiate(
      tokenPrefab.gameObject,
      tokenPosition,
      Quaternion.identity,
      transform
    ).GetComponent<PlayerToken>();

    if(token != null)
    {
      token.OnDeadStatusChange += (playerObj, status) => OnDeadStatusChange?.Invoke(tokenObjs.IndexOf(playerObj), status);
      token.OnVotelessStatusChange += (playerObj, status) => OnVotelessStatusChange?.Invoke(tokenObjs.IndexOf(playerObj), status);
      token.OnRoleChange += (playerObj, role) => OnRoleChange?.Invoke(tokenObjs.IndexOf(playerObj), role);
      token.OnReminderAdd += (playerObj) => OnReminderAdd?.Invoke(tokenObjs.IndexOf(playerObj));
      token.OnReminderRemove += (playerObj, reminderIndex) => OnReminderRemove?.Invoke(tokenObjs.IndexOf(playerObj), reminderIndex)?? false;

      token.OnRename += (playerObj) => OnRename?.Invoke(tokenObjs.IndexOf(playerObj));
      token.OnMove += (playerObj) => OnMove?.Invoke(tokenObjs.IndexOf(playerObj));
      token.OnSwap += (playerObj) => OnSwap?.Invoke(tokenObjs.IndexOf(playerObj));
      token.OnRemove += (playerObj) => OnRemove?.Invoke(tokenObjs.IndexOf(playerObj));
      token.OnStartNomination += (playerObj) => OnStartNomination?.Invoke(tokenObjs.IndexOf(playerObj));
      token.OnNominate += (playerObj) => OnNominate?.Invoke(tokenObjs.IndexOf(playerObj));

      token.Init(info, -angle);
      tokenObjs.Add(token);
    }
  }

  public void ReloadNightOrderInfo()
  {
    foreach(PlayerToken p in tokenObjs)
    {
      p.ReloadNightOrderInfo();
    }
  }

  public void AddPlayerToken(PlayerInfo info)
  {
    var tManager = TokensManager.Instance;

    PlayerToken tokenPrefab = tManager.playerTokenPrefab;
    if(tokenPrefab == null)
    {
      tokenPrefab = Resources.Load<PlayerToken>("PlayerToken");
      if(tokenPrefab == null) return;
    }

    CreatePlayerToken(tokenPrefab, info, 0.0f);

    ResetTokensPositions();
  }

  public void RemovePlayerToken(int playerIndex)
  {
    if(playerIndex >= tokenObjs.Count) return;

    PlayerToken tokenToRemove = tokenObjs[playerIndex];
    tokenObjs.RemoveAt(playerIndex);
    Destroy(tokenToRemove.gameObject);

    ResetTokensPositions();
  }

  public void ResetTokensPositions()
  {
    int tokenCount = tokenObjs.Count;
    for(int i = 0; i < tokenCount; i++)
    {
      float tokenAngle = 360.0f / tokenCount * i - 90.0f;
      tokenAngle *= -Mathf.Deg2Rad;

      Vector3 tokenPosition =
        transform.position +
        new Vector3(
          Mathf.Cos(tokenAngle),
          Mathf.Sin(tokenAngle),
          0.0f
        ) *
        tokenRadius;

      PlayerToken token = tokenObjs[i];
      token.transform.position = tokenPosition;
      token.tokenAngle = -tokenAngle;
      token.ReloadReminders();
    }
  }

  public void SetVisibilityState(bool state)
  {
    foreach(PlayerToken p in tokenObjs)
    {
      p.SetVisibilityState(state);
    }
  }

  public void SetNightOrderVisibility(bool state)
  {
    foreach(PlayerToken p in tokenObjs)
    {
      p.SetNightOrderVisibility(state);
    }
  }


  public void AddPlayerReminder(int playerIndex, ReminderInfo info)
  {
    tokenObjs[playerIndex].AddReminder(info);
  }


  public void CloseAllPlayersActionMenu()
  {
    foreach (var p in tokenObjs)
    {
      p.SetActionsMenuActive(false);
    }
  }
  public void ClosePlayerActionMenu(int playerIndex)
  {
    if(playerIndex >= tokenObjs.Count) return;
    tokenObjs[playerIndex].SetActionsMenuActive(false);
  }


  public void RemovePlayersRole()
  {
    foreach (PlayerToken p in tokenObjs)
    {
      p.ChangeRole("");
    }
  }
  public void RemovePlayersReminders()
  {
    foreach(PlayerToken p in tokenObjs)
    {
      p.RemoveAllReminders();
    }
  }
  public void ReviveAllPlayers()
  {
    foreach(PlayerToken p in tokenObjs)
    {
      p.SetDeadState(false);
    }
  }
  public void ResetGame()
  {
    RemovePlayersRole();
    RemovePlayersReminders();
    ReviveAllPlayers();
  }

  public void RemoveAllPlayers()
  {
    foreach(PlayerToken p in tokenObjs)
    {
      Destroy(p.gameObject);
    }
    tokenObjs.Clear();
  }

  public void RenamePlayer(int playerIndex, string newName)
  {
    if(playerIndex >= tokenObjs.Count) return;
    tokenObjs[playerIndex].Rename(newName);
  }


  public void SwapPlayers(int p1, int p2)
  {
    if(p1 >= tokenObjs.Count || p2 >= tokenObjs.Count) return;

    PlayerToken temp = tokenObjs[p1];
    tokenObjs[p1] = tokenObjs[p2];
    tokenObjs[p2] = temp;

    ResetTokensPositions();
  }

  public void MovePlayer(int p1, int p2)
  {
    if(p1 >= tokenObjs.Count || p2 >= tokenObjs.Count) return;

    PlayerToken temp = tokenObjs[p1];
    int direction = p1 < p2 ? 1 : -1;
    for(int i = p1; p1 < p2 ? i < p2 : i > p2; i += direction)
    {
      temp = tokenObjs[i];
      tokenObjs[i] = tokenObjs[i + direction];
      tokenObjs[i + direction] = temp;
    }

    ResetTokensPositions();
  }

  public void PlayerNominate(int p1, int p2)
  {

  }

  public void PlayerVoted(int p)
  {

  }


  public void AskForGroupAction(Texture2D img, PlayerActionDelegate confirmCallback,
    int? cancelPlayerIndex = null, Texture2D cancelImg = null, TokensManager.VoidDelegate cancelCallback = null)
  {
    int playerCount = tokenObjs.Count;
    for (int i = 0; i < playerCount; ++i)
    {
      if (cancelPlayerIndex != null && cancelImg != null &&
          cancelCallback != null && i == cancelPlayerIndex)
      {
        tokenObjs[i].AskForGroupAction(cancelImg, (playerObj) => cancelCallback());
        continue;
      }

      tokenObjs[i].AskForGroupAction(img, (playerObj) =>
        confirmCallback(tokenObjs.IndexOf(playerObj)));
    }
  }

  public void CancelGroupAction()
  {
    foreach(var p in tokenObjs)
    {
      p.HideGroupAction();
    }
  }
}