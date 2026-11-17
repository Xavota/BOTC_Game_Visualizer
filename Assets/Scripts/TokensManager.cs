using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct TeamCountModifier
{
  public string team;
  public string action;
  public int countModifier;
}

[System.Serializable]
public struct RoleData
{
  public string id;
  public string name;
  public string[] editions;
  public string team;
  public int firstNight;
  public string firstNightReminder;
  public int otherNight;
  public string otherNightReminder;
  public string[] reminders;
  public string[] remindersGlobal;
  public bool setup;
  public string ability;
  public TeamCountModifier[] teamCountModifiers;

}

[System.Serializable]
public struct FabledData
{
  public string id;
  public string firstNightReminder;
  public int otherNight;
  public string otherNightReminder;
  public string[] reminders;
  public bool setup;
  public string name;
  public string team;
  public string ability;
  public TeamCountModifier[] teamCountModifiers;
}

[System.Serializable]
public struct EditionInfo
{
  public string id;
  public string name;
  public string author;
}

[System.Serializable]
public struct RoleInfo
{
  public RoleInfo(string id)
  {
    this.id = id;
  }
  public string id;
}

[System.Serializable]
public struct ReminderInfo
{
  public ReminderInfo(string role, string name)
  {
    this.role = role;
    this.name = name;
  }

  public string role;
  public string name;
}

[System.Serializable]
public struct PlayerInfo
{
  public string name;
  public string role;

  public ReminderInfo[] reminders;

  public bool isVoteless;
  public bool isDead;
}

[System.Serializable]
public struct GameStateInfo
{
  public string[] bluffs;
  public EditionInfo edition;

  public RoleInfo[] roles;
  public RoleRename[] renames;
  public RoleInfo[] fabled;
  public PlayerInfo[] players;
}

[System.Serializable]
public struct RoleRename
{
  public string role;
  public string rename;
}

[System.Serializable]
public struct EditionData
{
  public string id;
  public string name;
  public string author;
  public string description;
  public string level;
  public string[] roles;
  public RoleRename[] renames;
  bool isOfficial;
}

public class TokensManager : MonoBehaviour
{
  public delegate void VoidDelegate();
  public delegate void BoolDelegate(bool state);
  public delegate bool BoolReturnDelegate(string id);
  public delegate void IDDelegate(string id);
  public delegate void ReminderDelegate(ReminderInfo reminder);
  public delegate void NameDelegate(string id);
  public delegate void DemonBluffChangedDelegate(int index, string id);

  private static TokensManager instance;

  public static TokensManager Instance
  {
    get
    {
      if(instance == null)
      {
        instance = FindAnyObjectByType<TokensManager>();
        if(instance == null)
        {
          GameObject obj = new GameObject("TokensManager");
          instance = obj.AddComponent<TokensManager>();
        }
      }
      return instance;
    }
  }


  [System.Serializable]
  public struct RoleDataList
  {
    public RoleData[] array;
  }

  [System.Serializable]
  public struct FabledDataList
  {
    public FabledData[] array;
  }

  [System.Serializable]
  public struct EditionDataList
  {
    public EditionData[] array;
  }

  [HideInInspector] public Dictionary<string, RoleData> rolesData = new();
  [HideInInspector] public Dictionary<string, FabledData> fabledData = new();
  [HideInInspector] public List<string> globalReminders = new();
  [HideInInspector] public Dictionary<string, EditionData> editionsData = new();

  private string gamestatePath;
  public GameStateInfo gamestate;

  public SettingsMenu settingsMenuObj;
  public TokenRing tokenRingPrefab;

  public BasicRoleToken basicRoleTokenPrefab;
  public BasicRoleToken fabledTokenPrefab;
  public Token tokenPrefab;
  public PlayerToken playerTokenPrefab;
  public Reminder reminderPrefab;
  public EditionBtn editionBtnPrefab;

  public FabledArea fabledAreaPrefab;
  public DemonBluffsArea demonBluffsAreaPrefab;

  public List<Color> rolesColor;
  public RolesPopup rolesPopup;
  public ConfirmationPopup confirmationPopup;
  public RemindersPopup remindersPopup;
  public NamePopup namePopup;
  public EditionsPopup editionsPopup;
  public PlayerReminderPopup playerReminderPopup;

  public ShowPlayerReminder showPlayerReminder;
  public ShowDemonBluffs showDemonBlufffs;


  [SerializeField] private TokenRing tokenRingObj;
  [SerializeField] private FabledArea fabledAreaObj;
  [SerializeField] private DemonBluffsArea demonBluffsAreaObj;


  [SerializeField] private GameState gameStateObj;
  [SerializeField] private GameObject nightTimeBackgroundImg;


  [SerializeField] private Texture2D cancelActionIcon;
  [SerializeField] private Texture2D moveActionIcon;
  [SerializeField] private Texture2D swapActionIcon;
  [SerializeField] private Texture2D nominateActionIcon;
  [SerializeField] private Texture2D voteActionIcon;


  [SerializeField] private ReferenceSheetViewer referenceSheetViewer;
  [SerializeField] private ReferenceSheet bigReferenceSheet;
  [SerializeField] private ReferenceSheetExporter referenceSheetExporter;


  public void Start()
  {
    ReadRolesJson("jsons/roles");
    ReadFabledJson("jsons/fabled");
    ReadEditionsJson("jsons/editions");

    gamestatePath = Path.Combine(
      Application.persistentDataPath,
      "gamestate.json"
    );
    LoadGameState();

    globalReminders.Add("evil");
    globalReminders.Add("good");
    globalReminders.Add("custom");
    globalReminders.Add("plus");

    CreateTokenRing();
    CreateFabledArea();
    CreateDemonBluffsArea();

    SetupSettingsMenu();
    SetupGameStateObj();
    SetupPlayerReminderPopup();
  }

  public void LoadGameState()
  {
    string json;

    if(File.Exists(gamestatePath))
    {
      // Load existing save
      json = File.ReadAllText(gamestatePath);
    }
    else
    {
      // Load default state from Resources
      TextAsset defaultState =
          Resources.Load<TextAsset>("jsons/gamestate");

      if(defaultState == null)
      {
        Debug.LogError("Default game state not found!");
        return;
      }

      json = defaultState.text;
    }

    gamestate = JsonUtility.FromJson<GameStateInfo>(json);

    // Create a persistent save on first launch
    if(!File.Exists(gamestatePath))
    {
      SaveGameState();
    }
  }

  public void SaveGameState()
  {
    string json = JsonUtility.ToJson(gamestate, true);
    File.WriteAllText(gamestatePath, json);
  }

  private void ReadRolesJson(string jsonFileName)
  {
    TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
    if(jsonFile != null)
    {
      string jsonString = jsonFile.text;
      var roleList =
        JsonUtility.FromJson<RoleDataList>("{\"array\":" + jsonString + "}")
        .array.ToList();

      foreach(var role in roleList)
      {
        RoleData r = role;
        if(r.remindersGlobal == null)
        {
          r.remindersGlobal = Array.Empty<string>();
        }
        rolesData.Add(r.id, r);
      }
    }
  }

  private void ReadFabledJson(string jsonFileName)
  {
    TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
    if(jsonFile != null)
    {
      string jsonString = jsonFile.text;
      var fabledList =
        JsonUtility.FromJson<FabledDataList>("{\"array\":" + jsonString + "}")
        .array.ToList();

      foreach(var fabled in fabledList)
      {
        FabledData f = fabled;
        fabledData.Add(f.id, f);
      }
    }
  }

  private void ReadEditionsJson(string jsonFileName)
  {
    TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
    if(jsonFile != null)
    {
      string jsonString = jsonFile.text;
      var editionsList =
        JsonUtility.FromJson<EditionDataList>("{\"array\":" + jsonString + "}")
        .array.ToList();

      foreach(var edition in editionsList)
      {
        EditionData e = edition;
        editionsData.Add(e.id, e);
      }

      foreach(var r in rolesData.Values)
      {
        foreach(var e in r.editions)
        {
          if(!editionsData.ContainsKey(e)) continue;

          var eData = editionsData[e];
          List<string> eRoles = eData.roles.ToList();
          if(eRoles.Contains(r.id)) continue;
          eRoles.Add(r.id);
          eData.roles = eRoles.ToArray();
          editionsData[e] = eData;
        }
      }
    }
  }


  private void CreateTokenRing()
  {
    if(tokenRingPrefab == null)
    {
      tokenRingPrefab = Resources.Load<TokenRing>("TokenRing");
    }

    if(tokenRingObj == null)
    {
      var canvas = GameObject.Find("Canvas");

      if(tokenRingPrefab == null)
      {
        GameObject tokenRingGameObj = new();
        tokenRingGameObj.transform.position = transform.position;
        tokenRingGameObj.transform.rotation = Quaternion.identity;
        tokenRingGameObj.transform.parent = canvas.transform;
        tokenRingGameObj.name = "TokenRing";
        tokenRingObj = tokenRingGameObj.AddComponent<TokenRing>();
      }
      else
      {
        tokenRingObj = Instantiate(
          tokenRingPrefab.gameObject,
          transform.position,
          Quaternion.identity,
          canvas.transform
        ).GetComponent<TokenRing>();
        tokenRingObj.gameObject.name = "TokenRing";
      }
    }

    if(tokenRingObj != null)
    {
      tokenRingObj.OnDeadStatusChange += (playerIndex, status) =>
      {
        List<PlayerInfo> players = gamestate.players.ToList();
        PlayerInfo player = players[playerIndex];

        player.isDead = status;
        if(!status) player.isVoteless = false;

        players[playerIndex] = player;
        gamestate.players = players.ToArray();
        SaveGameState();
        UpdatePlayerCount();
      };
      tokenRingObj.OnVotelessStatusChange += (playerIndex, status) =>
      {
        List<PlayerInfo> players = gamestate.players.ToList();
        PlayerInfo player = players[playerIndex];

        player.isVoteless = status;

        players[playerIndex] = player;
        gamestate.players = players.ToArray();
        SaveGameState();
        UpdatePlayerCount();
      };
      tokenRingObj.OnRoleChange += (playerIndex, role) =>
      {
        List<PlayerInfo> players = gamestate.players.ToList();
        PlayerInfo player = players[playerIndex];

        player.role = role;

        players[playerIndex] = player;
        gamestate.players = players.ToArray();
        SaveGameState();
        UpdatePlayerCount();

        if(tokenRingObj != null) tokenRingObj.ReloadNightOrderInfo();
        if(fabledAreaObj != null) fabledAreaObj.ReloadNightOrderInfo();
      };
      tokenRingObj.OnReminderAdd += (playerIndex) =>
      {
        AskForReminder(
          "Select a reminder to add:",
          (rem) =>
          {
            List<PlayerInfo> players = gamestate.players.ToList();
            PlayerInfo player = players[playerIndex];

            List<ReminderInfo> reminders = player.reminders.ToList();
            reminders.Add(rem);
            player.reminders = reminders.ToArray();

            players[playerIndex] = player;
            gamestate.players = players.ToArray();
            SaveGameState();

            tokenRingObj.AddPlayerReminder(playerIndex, rem);
          }
        );
      };
      tokenRingObj.OnReminderRemove += (playerIndex, reminderIndex) =>
      {
        List<PlayerInfo> players = gamestate.players.ToList();
        PlayerInfo player = players[playerIndex];

        List<ReminderInfo> reminders = player.reminders.ToList();
        reminders.RemoveAt(reminderIndex);
        player.reminders = reminders.ToArray();

        players[playerIndex] = player;
        gamestate.players = players.ToArray();
        SaveGameState();

        return true;
      };

      // ACTIONS MENU
      tokenRingObj.OnRename += (playerIndex) =>
      {
        tokenRingObj.CloseAllPlayersActionMenu();
        AskForPlayerName("Give a new name for the player:", (newName) =>
        {
          List<PlayerInfo> players = gamestate.players.ToList();
          PlayerInfo player = players[playerIndex];

          player.name = newName;

          players[playerIndex] = player;
          gamestate.players = players.ToArray();
          SaveGameState();

          tokenRingObj.RenamePlayer(playerIndex, newName);
        });
      };
      tokenRingObj.OnMove += (p1) =>
      {
        tokenRingObj.CloseAllPlayersActionMenu();
        tokenRingObj.AskForGroupAction(
          moveActionIcon,
          (p2) =>
          {
            List<PlayerInfo> players = gamestate.players.ToList();

            PlayerInfo temp = players[p1];
            int direction = p1 < p2 ? 1 : -1;
            for(int i = p1; p1 < p2 ? i < p2 : i > p2; i += direction)
            {
              temp = players[i];
              players[i] = players[i + direction];
              players[i + direction] = temp;
            }

            gamestate.players = players.ToArray();
            SaveGameState();

            tokenRingObj.MovePlayer(p1, p2);
            tokenRingObj.CancelGroupAction();
          },
          p1,
          cancelActionIcon,
          () =>
          {
            tokenRingObj.CancelGroupAction();
          }
        );
      };
      tokenRingObj.OnSwap += (p1) =>
      {
        tokenRingObj.CloseAllPlayersActionMenu();
        tokenRingObj.AskForGroupAction(
          swapActionIcon,
          (p2) =>
          {
            List<PlayerInfo> players = gamestate.players.ToList();

            PlayerInfo temp = players[p1];
            players[p1] = players[p2];
            players[p2] = temp;

            gamestate.players = players.ToArray();
            SaveGameState();

            tokenRingObj.SwapPlayers(p1, p2);
            tokenRingObj.CancelGroupAction();
          },
          p1,
          cancelActionIcon,
          () =>
          {
            tokenRingObj.CancelGroupAction();
          }
        );
      };
      tokenRingObj.OnRemove += (playerIndex) =>
      {
        tokenRingObj.CloseAllPlayersActionMenu();
        AskForConfirmation(
          () =>
          {
            List<PlayerInfo> players = gamestate.players.ToList();
            players.RemoveAt(playerIndex);
            gamestate.players = players.ToArray();
            SaveGameState();
            UpdatePlayerCount();

            if(tokenRingObj != null) tokenRingObj.RemovePlayerToken(playerIndex);
          },
          () => { },

          "Are you sure you want to remove this player?"
        );
      };
      tokenRingObj.OnStartNomination += (p1) =>
      {
        tokenRingObj.CloseAllPlayersActionMenu();
        return;
        tokenRingObj.AskForGroupAction(
          nominateActionIcon,
          (p2) =>
          {
            tokenRingObj.CancelGroupAction();
            tokenRingObj.AskForGroupAction(
              voteActionIcon,
              (p2) =>
              {
                tokenRingObj.CancelGroupAction();
              }
            );
          },
          p1,
          cancelActionIcon,
          () =>
          {
            tokenRingObj.CancelGroupAction();
          }
        );
      };
      tokenRingObj.OnNominate += (playerIndex) =>
      {
        tokenRingObj.CloseAllPlayersActionMenu();
      };

      tokenRingObj.Init(gamestate.players.ToList());
    }
  }

  private void CreateFabledArea()
  {
    if(fabledAreaObj != null)
    {
      fabledAreaObj.gameObject.SetActive(gamestate.fabled.Length > 0);
      fabledAreaObj.Init(gamestate.fabled.ToList());
      fabledAreaObj.OnTokenRemoved += OnFabledTokenRemoved;
      return;
    }

    fabledAreaPrefab ??= Resources.Load<FabledArea>("FabledArea");
    if(fabledAreaPrefab == null) return;

    var canvas = GameObject.Find("Canvas");

    fabledAreaObj = Instantiate(
      fabledAreaPrefab.gameObject,
      canvas.transform
    ).GetComponent<FabledArea>();
    fabledAreaObj.gameObject.name = "FabledArea";
    fabledAreaObj.Init(gamestate.fabled.ToList());
  }

  private void CreateDemonBluffsArea()
  {
    if(demonBluffsAreaObj == null)
    {
      demonBluffsAreaPrefab ??= Resources.Load<DemonBluffsArea>("DemonBluffsArea");
      if(demonBluffsAreaPrefab == null) return;

      var canvas = GameObject.Find("Canvas");

      demonBluffsAreaObj = Instantiate(
        demonBluffsAreaPrefab.gameObject,
        canvas.transform
      ).GetComponent<DemonBluffsArea>();
      demonBluffsAreaObj.gameObject.name = "DemonBluffsArea";
    }

    if(demonBluffsAreaObj != null)
    {
      demonBluffsAreaObj.OnBluffChanged += (index, id) =>
      {
        List<string> bluffs = gamestate.bluffs.ToList();
        if (index >= bluffs.Count)
        {
          for (int i = bluffs.Count; i <= index; ++i)
          {
            bluffs.Add("");
          }
        }
        bluffs[index] = id;
        gamestate.bluffs = bluffs.ToArray();
        SaveGameState();
      };
      demonBluffsAreaObj.OnExpand += ShowDemonBluffs;
      demonBluffsAreaObj.Init(gamestate.bluffs.ToList());
      return;
    }
  }


  private void SetupSettingsMenu()
  {
    if(settingsMenuObj == null) return;

    // TODO: GUARDAR EL GAMESTATE CONSTANTEMENTE
    //
    // GRIMOIRE
    //
    settingsMenuObj.OnResetGame += () =>
    {
      if(tokenRingObj == null) return;

      AskForConfirmation(
        () =>
        {
          List<PlayerInfo> players = gamestate.players.ToList();
          int playersCount = players.Count;
          for (int i = 0; i < playersCount; ++i)
          {
            var p = players[i];
            p.role = "";
            p.reminders = (new List<ReminderInfo>()).ToArray();
            p.isDead = false;
            p.isVoteless = false;
            players[i] = p;
          }
          gamestate.players = players.ToArray();

          gamestate.bluffs = (new List<string>()).ToArray();
          gamestate.fabled = (new List<RoleInfo>()).ToArray();
          SaveGameState();

          tokenRingObj.ResetGame();
          if(demonBluffsAreaObj != null) demonBluffsAreaObj.RemoveRoles();
          if(fabledAreaObj != null) fabledAreaObj.ClearTokens();
          fabledAreaObj.gameObject.SetActive(false);
        },
        () => {},

        "Do you want to reset the game state (keep only the players)?"
      );
    };
    settingsMenuObj.OnTokensVisibilityChange += (state) =>
    {
      if(tokenRingObj != null) tokenRingObj.SetVisibilityState(state);
      if(demonBluffsAreaObj != null) demonBluffsAreaObj.gameObject.SetActive(state);
      if(gameStateObj != null) gameStateObj.SetInfoVisibility(state);
    };
    settingsMenuObj.OnNightTimeChange += (state) =>
    {
      if(gameStateObj != null) gameStateObj.SetNightPhaseState(state);
      if(nightTimeBackgroundImg != null) nightTimeBackgroundImg.SetActive(state);
    };
    settingsMenuObj.OnNightOrderVisibilityChange += (state) =>
    {
      if(tokenRingObj != null) tokenRingObj.SetNightOrderVisibility(state);
    };

    //
    // PLAYERS
    //
    settingsMenuObj.OnPlayerAdded += StartAddingPlayer;
    // Randomize seats
    settingsMenuObj.OnRemoveAllPlayers += () =>
    {
      if(tokenRingObj == null) return;

      AskForConfirmation(
        () =>
        {
          gamestate.players = (new List<PlayerInfo>()).ToArray();
          UpdatePlayerCount();
          SaveGameState();
          tokenRingObj.RemoveAllPlayers();
        },
        () => { },

        "Are you sure you want to remove all players?"
      );
    };
    settingsMenuObj.OnShowReminder += AskForPlayerReminder;

    //
    // CHARACTERS
    //
    settingsMenuObj.OnSelectEdition += SelectEdition;
    // Choose and assign characters
    settingsMenuObj.OnFabledAdded += StartAddingFabled;
    settingsMenuObj.OnRemoveAllCharacters += RemoveAllCharacters;

    //
    // HELP
    //
    settingsMenuObj.OnShowReferenceSheet += () =>
    {
      ShowReferenceSheet(() => { },
      () =>
      {
        DownloadReferenceSheet();
      });
    };
    // Night order sheet
    // Download gamestate
  }

  private void SetupGameStateObj()
  {
    if(gameStateObj == null) return;

    gameStateObj.SetPlayersCount(
      gamestate.players.Length,
      gamestate.players.Where(p => p.isDead).Count(),
      gamestate.players.Where(p => !p.isVoteless).Count(),
      GetTeamsCount(),
      GetTeamsCountMod()
    );
  }

  private void SetupPlayerReminderPopup()
  {
    if(playerReminderPopup == null) return;

    playerReminderPopup.OnShowYouAreRoleReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      AskForRole(
        "Select a role for \"You are\" reminder:",
        (id) =>
        {
          ShowPlayerReminder("You are", id, null);
        }
      );
    };
    playerReminderPopup.OnShowSelectedYouReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      AskForRole(
        "Select a role for \"This character selected you\" reminder:",
        (id) =>
        {
          ShowPlayerReminder("This character selected you", id, null);
        }
      );
    };
    playerReminderPopup.OnShowCharactersInPlayReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      AskForRole(
        "Select a role for \"These characters are in play\" reminder:",
        (id) =>
        {
          ShowPlayerReminder("These characters are in play", id, null);
        }
      );
    };
    playerReminderPopup.OnShowCustomWithRoleReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      AskForPlayerName(
        "Write a custom reminder title:",
        (text) =>
        {
          AskForRole(
            "Select a role for \"" + text + "\" reminder:",
            (id) =>
            {
              ShowPlayerReminder(text, id, null);
            }
          );
        }
      );
    };
    playerReminderPopup.OnShowOnlyRoleReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      AskForRole(
        "Select a role for reminder:",
        (id) =>
        {
          ShowPlayerReminder(null, id, null);
        }
      );
    };

    playerReminderPopup.OnShowYouAreReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      AskForReminder(
        "Select a reminder for \"You are\" reminder:",
        (reminder) =>
        {
          ShowPlayerReminder("You are", null, reminder);
        }
      );
    };
    playerReminderPopup.OnShowCustomWithReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      AskForPlayerName(
        "Write a custom reminder title:",
        (text) =>
        {
          AskForReminder(
            "Select a reminder for \"" + text + "\" reminder:",
            (reminder) =>
            {
              ShowPlayerReminder(text, null, reminder);
            }
          );
        }
      );
    };
    playerReminderPopup.OnShowOnlyReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      AskForReminder(
        "Select a reminder to show:",
        (reminder) =>
        {
          ShowPlayerReminder(null, null, reminder);
        }
      );
    };

    playerReminderPopup.OnShowAttackedReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      ShowPlayerReminder("Attacked", null, null);
    };
    playerReminderPopup.OnShowSelectPlayerReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      ShowPlayerReminder("Select player(s)", null, null);
    };
    playerReminderPopup.OnShowSelectCharacterReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      ShowPlayerReminder("Select Character(s)", null, null);
    };
    playerReminderPopup.OnShowCustomWithoutRoleReminder += () =>
    {
      playerReminderPopup.gameObject.SetActive(false);
      AskForPlayerName(
        "Write a custom reminder title:",
        (text) =>
        {
          ShowPlayerReminder(text, null, null);
        }
      );
    };
  }


  private bool OnFabledTokenRemoved(string id)
  {
    var fabledList = gamestate.fabled.ToList();
    fabledList.Remove(new RoleInfo(id));
    gamestate.fabled = fabledList.ToArray();
    SaveGameState();
    UpdatePlayerCount();

    if(tokenRingObj != null) tokenRingObj.ReloadNightOrderInfo();
    if(fabledAreaObj != null) fabledAreaObj.ReloadNightOrderInfo();

    if(gamestate.fabled.Length == 0)
    {
      fabledAreaObj.gameObject.SetActive(false);
    }

    return true;
  }

  public void AddFabled(string id)
  {
    var fabledList = gamestate.fabled.ToList();
    fabledList.Add(new RoleInfo(id));
    gamestate.fabled = fabledList.ToArray();
    SaveGameState();
    UpdatePlayerCount();

    fabledAreaObj.AddToken(id);
    fabledAreaObj.gameObject.SetActive(true);
  }


  private void ChangeEditionImg(string id)
  {
    if(gameStateObj == null) return;

    var img = GetEditionImg(id);
    gameStateObj.ChangeEditionImg(img);
  }


  public void AskForConfirmation(VoidDelegate confirmCallback,
                                 VoidDelegate cancelCallback, string title)
  {
    if(confirmationPopup == null)
    {
      Debug.LogWarning("No confirmation popup");
      return;
    }

    confirmationPopup.Init(title);
    confirmationPopup.gameObject.SetActive(true);

    VoidDelegate handler = null;
    VoidDelegate handler2 = null;

    handler = () =>
    {
      confirmationPopup.OnAccept -= handler;
      confirmationPopup.OnClosed -= handler2;
      confirmationPopup.gameObject.SetActive(false);
      confirmCallback();
    };

    handler2 = () =>
    {
      confirmationPopup.OnClosed -= handler2;
      confirmationPopup.OnAccept -= handler;
      confirmationPopup.gameObject.SetActive(false);
      cancelCallback();
    };

    confirmationPopup.OnAccept += handler;
    confirmationPopup.OnClosed += handler2;
  }
  public void AskForRole(string title, IDDelegate callback, bool emptyRole = true,
                         bool notInPlay = false, bool fabled = false)
  {
    if(rolesPopup == null)
    {
      Debug.LogWarning("No roles popup");
      return;
    }

    var inPlayCharactesList = gamestate.players.Select(r => r.role);
    var inPlayFabledList = gamestate.fabled.Select(r => r.id);
    List<string> rolesList = fabled ?
      fabledData
        .Where(r => !notInPlay ||
                    !inPlayFabledList.Contains(r.Value.id))
        .Select(r => r.Value.id)
        .ToList() :
      gamestate.roles
        .Where(r => !notInPlay ||
                    !inPlayCharactesList.Contains(r.id))
        .Select(r => r.id)
        .ToList();

    rolesList = OrderByName(rolesList);
    rolesList = OrderByNight(rolesList);
    rolesList = OrderByTeam(rolesList);

    if(emptyRole)
    {
      rolesList.Add("");
    }

    rolesPopup.Init(rolesList, title);
    rolesPopup.gameObject.SetActive(true);

    IDDelegate handler = null;
    VoidDelegate handler2 = null;

    handler = (id) =>
    {
      rolesPopup.OnTokenSelected -= handler;
      rolesPopup.OnClosed -= handler2;
      rolesPopup.gameObject.SetActive(false);
      rolesPopup.ClearTokens();
      callback(id);
    };

    handler2 = () =>
    {
      rolesPopup.OnClosed -= handler2;
      rolesPopup.OnTokenSelected -= handler;
      rolesPopup.gameObject.SetActive(false);
      rolesPopup.ClearTokens();
    };

    rolesPopup.OnTokenSelected += handler;
    rolesPopup.OnClosed += handler2;
  }
  public void AskForReminder(string title, ReminderDelegate callback)
  {
    List<ReminderInfo> inPlayReminders = new();
    List<string> addedRoles = new();
    foreach (var p in gamestate.players)
    {
      if(!rolesData.ContainsKey(p.role) || addedRoles.Contains(p.role)) continue;

      RoleData roleInfo = rolesData[p.role];
      foreach (var reminder in roleInfo.reminders)
      {
        inPlayReminders.Add(new ReminderInfo(p.role, reminder));
      }
      foreach(var reminder in roleInfo.remindersGlobal)
      {
        inPlayReminders.Add(new ReminderInfo(p.role, reminder));
      }

      addedRoles.Add(p.role);
    }
    foreach(var f in gamestate.fabled)
    {
      if(!fabledData.ContainsKey(f.id) || addedRoles.Contains(f.id)) continue;

      FabledData fabledInfo = fabledData[f.id];
      foreach(var reminder in fabledInfo.reminders)
      {
        inPlayReminders.Add(new ReminderInfo(f.id, reminder));
      }

      addedRoles.Add(f.id);
    }
    foreach (var r in gamestate.roles)
    {
      if(!rolesData.ContainsKey(r.id) || addedRoles.Contains(r.id)) continue;

      RoleData roleInfo = rolesData[r.id];
      foreach(var reminder in roleInfo.remindersGlobal)
      {
        inPlayReminders.Add(new ReminderInfo(r.id, reminder));
      }

      addedRoles.Add(r.id);
    }

    inPlayReminders = OrderReminders(inPlayReminders);
    inPlayReminders.Add(new ReminderInfo("good", "Good"));
    inPlayReminders.Add(new ReminderInfo("evil", "Evil"));
    inPlayReminders.Add(new ReminderInfo("custom", "Custom Note"));

    remindersPopup.Init(inPlayReminders, title);
    remindersPopup.gameObject.SetActive(true);

    ReminderDelegate handler = null;
    VoidDelegate handler2 = null;

    handler = (id) =>
    {
      remindersPopup.OnTokenSelected -= handler;
      remindersPopup.OnClosed -= handler2;
      remindersPopup.gameObject.SetActive(false);
      remindersPopup.ClearTokens();
      callback(id);
    };

    handler2 = () =>
    {
      remindersPopup.OnClosed -= handler2;
      remindersPopup.OnTokenSelected -= handler;
      remindersPopup.gameObject.SetActive(false);
      remindersPopup.ClearTokens();
    };

    remindersPopup.OnTokenSelected += handler;
    remindersPopup.OnClosed += handler2;
  }
  public void AskForPlayerName(string title, NameDelegate callback)
  {
    if(namePopup == null)
    {
      Debug.LogWarning("No name popup");
      return;
    }

    namePopup.gameObject.SetActive(true);
    namePopup.Focus();

    NameDelegate handler = null;
    VoidDelegate handler2 = null;

    handler = (name) =>
    {
      namePopup.OnNameChosen -= handler;
      namePopup.OnClosed -= handler2;
      namePopup.gameObject.SetActive(false);
      callback(name);
    };

    handler2 = () =>
    {
      namePopup.OnClosed -= handler2;
      namePopup.OnNameChosen -= handler;
      namePopup.gameObject.SetActive(false);
    };

    namePopup.SetTitle(title);
    namePopup.OnNameChosen += handler;
    namePopup.OnClosed += handler2;
  }
  public void AskForEdition(IDDelegate callback)
  {
    if(editionsPopup == null)
    {
      Debug.LogWarning("No editions popup");
      return;
    }

    var editionsList = editionsData.Keys.ToList();
    editionsList.Add("custom");
    editionsPopup.Init(editionsList);
    editionsPopup.gameObject.SetActive(true);

    IDDelegate handler = null;
    VoidDelegate handler2 = null;

    handler = (id) =>
    {
      editionsPopup.OnEditionSelected -= handler;
      editionsPopup.OnClosed -= handler2;
      editionsPopup.gameObject.SetActive(false);
      editionsPopup.ClearBtns();
      callback(id);
    };

    handler2 = () =>
    {
      editionsPopup.OnClosed -= handler2;
      editionsPopup.OnEditionSelected -= handler;
      editionsPopup.gameObject.SetActive(false);
      editionsPopup.ClearBtns();
    };

    editionsPopup.OnEditionSelected += handler;
    editionsPopup.OnClosed += handler2;
  }
  public void ShowReferenceSheet(VoidDelegate closeCallback, 
                                 VoidDelegate downloadCallback)
  {
    if(referenceSheetViewer == null)
    {
      Debug.LogWarning("No reference sheet viewer");
      return;
    }

    var roles = OrderByName(gamestate.roles.Select(r => r.id).ToList());
    roles = OrderByNight(roles);
    roles = OrderByTeam(roles);
    referenceSheetViewer.Init(gamestate.edition.name, gamestate.edition.author, SeparateByTeam(roles));
    referenceSheetViewer.gameObject.SetActive(true);

    VoidDelegate handler = null;
    VoidDelegate handler2 = null;

    handler = () =>
    {
      referenceSheetViewer.OnClosed -= handler;
      referenceSheetViewer.OnDownload -= handler2;
      referenceSheetViewer.gameObject.SetActive(false);
      closeCallback();
    };

    handler2 = () =>
    {
      referenceSheetViewer.OnClosed -= handler;
      referenceSheetViewer.OnDownload -= handler2;
      referenceSheetViewer.gameObject.SetActive(false);
      downloadCallback();
    };

    referenceSheetViewer.OnClosed += handler;
    referenceSheetViewer.OnDownload += handler2;
  }
  public void AskForPlayerReminder()
  {
    if(playerReminderPopup == null)
    {
      Debug.LogWarning("No player reminder popup");
      return;
    }

    playerReminderPopup.gameObject.SetActive(true);

    VoidDelegate handler = null;
    handler = () =>
    {
      playerReminderPopup.OnClose -= handler;
      playerReminderPopup.gameObject.SetActive(false);
    };

    playerReminderPopup.OnClose += handler;
  }

  public void ShowPlayerReminder(string title, string role, ReminderInfo? reminder)
  {
    if(showPlayerReminder == null)
    {
      Debug.LogWarning("No player reminder for showing");
      return;
    }

    showPlayerReminder.gameObject.SetActive(true);

    VoidDelegate handler = null;
    handler = () =>
    {
      showPlayerReminder.OnClose -= handler;
      showPlayerReminder.gameObject.SetActive(false);
    };

    showPlayerReminder.OnClose += handler;
    showPlayerReminder.Init(title, role, reminder);
  }
  public void ShowDemonBluffs()
  {
    if(showDemonBlufffs == null)
    {
      Debug.LogWarning("No demon bluff expanded area");
      return;
    }

    showDemonBlufffs.gameObject.SetActive(true);

    VoidDelegate handler = null;
    handler = () =>
    {
      showDemonBlufffs.OnClose -= handler;
      showDemonBlufffs.gameObject.SetActive(false);
    };

    showDemonBlufffs.OnClose += handler;
    showDemonBlufffs.Init(gamestate.bluffs.ToList());
  }

  public void SelectEdition()
  {
    AskForEdition((id) =>
    {
      if(id == "custom")
      {
        string[] paths = StandaloneFileBrowser.OpenFilePanel(
          "Select JSON File",
          "",
          "json",
          false
        );

        // User canceled
        if(paths == null || paths.Length == 0)
          return;

        try
        {
          string json = File.ReadAllText(paths[0]);

          var editionInfo = JsonLoader.ParseEdition(json);

          if(editionInfo == null || editionInfo.Length < 2) return;

          gamestate.edition.id = id;
          gamestate.edition.name = editionInfo[0];
          gamestate.edition.author = editionInfo[1];

          gamestate.roles = editionInfo.ToList().GetRange(2, editionInfo.Length - 2)
            .Select((r) => new RoleInfo(r)).ToArray();
          gamestate.renames = null;
          SaveGameState();

          ChangeEditionImg(id);
        }
        catch(Exception e)
        {
          Debug.LogError(
              $"Failed to load JSON: {e.Message}"
          );
        }

        return;
      }

      var editionData = GetEditionData(id);

      if(editionData == null) return;

      gamestate.edition.id = id;
      gamestate.edition.name = editionData.Value.name;
      gamestate.edition.author = editionData.Value.author;

      gamestate.roles = editionData.Value.roles
        .Select((r) => new RoleInfo(r)).ToArray();
      gamestate.renames = editionData.Value.renames;
      SaveGameState();

      ChangeEditionImg(id);
    });
  }
  // Choose and assign characters
  public void StartAddingFabled()
  {
    AskForRole(
      "Select a fabled for the game:",
      (id) =>
      {
        AddFabled(id);
      }, 
      false, true, true
    );
  }
  public void RemoveAllCharacters()
  {
    if(tokenRingObj == null) return;

    AskForConfirmation(
      () =>
      {
        List<PlayerInfo> players = gamestate.players.ToList();
        int playersCount = players.Count;
        for(int i = 0; i < playersCount; ++i)
        {
          var p = players[i];
          p.role = "";
          players[i] = p;
        }
        gamestate.players = players.ToArray();
        SaveGameState();

        tokenRingObj.RemovePlayersRole();
      },
      () => { },

      "Are you sure you want to remove all player's roles?"
    );
  }


  public void DownloadReferenceSheet()
  {
    var roles = OrderByName(gamestate.roles.Select(r => r.id).ToList());
    roles = OrderByNight(roles);
    roles = OrderByTeam(roles);
    bigReferenceSheet.Init(gamestate.edition.name, gamestate.edition.author, SeparateByTeam(roles));
    referenceSheetExporter.ExportSheet(gamestate.edition.id + "RS");
    bigReferenceSheet.Init(gamestate.edition.name, gamestate.edition.author, SeparateByTeam(roles), true);
    referenceSheetExporter.ExportSheet(gamestate.edition.id + "RS_Travellers");
  }


  public RoleData? GetRoleData(string id)
  {
    return rolesData.ContainsKey(id) ? rolesData[id] : null;
  }
  public FabledData? GetFabledData(string id)
  {
    return fabledData.ContainsKey(id) ? fabledData[id] : null;
  }
  public string GetRename(string id)
  {
    var renameIndex = gamestate.renames?.ToList().FindIndex(r => r.role == id)?? -1;
    if(renameIndex != -1)
    {
      return gamestate.renames[renameIndex].rename;
    }

    return null;
  }

  public Texture2D GetRoleImage(string id)
  {
    var role = GetRoleData(id);
    if(role != null)
    {
      return Resources.Load<Texture2D>("Images/icons/" + role.Value.id);
    }

    return null;
  }

  public Texture2D GetFabledImage(string id)
  {
    var fabled = GetFabledData(id);
    if(fabled != null)
    {
      return Resources.Load<Texture2D>("Images/icons/" + fabled.Value.id);
    }

    return null;
  }

  public Texture2D GetReminderImage(string role, string name)
  {
    if(globalReminders.Contains(role))
    {
      return Resources.Load<Texture2D>("Images/icons/" + role);
    }

    var roleImg = GetRoleImage(role);
    if(roleImg != null) return roleImg;
    var fabledImg = GetFabledImage(role);
    if(fabledImg != null) return fabledImg;

    return null;
  }

  public bool ReminderExist(string role, string name)
  {
    return globalReminders.Contains(role) ||
           (
             rolesData.ContainsKey(role) &&
             (
               rolesData[role].reminders.Contains(name) ||
               rolesData[role].remindersGlobal.Contains(name)
             )
           ) ||
           (
             fabledData.ContainsKey(role) &&
             fabledData[role].reminders.Contains(name)
           );
  }


  public Color? GetRoleColor(string id)
  {
    if(!rolesData.ContainsKey(id)) return null;
    string team = rolesData[id].team;
    switch(team)
    {
      case "traveler":
        return rolesColor[0];
      case "townsfolk":
        return rolesColor[1];
      case "outsider":
        return rolesColor[2];
      case "minion":
        return rolesColor[3];
      case "demon":
        return rolesColor[4];
      default:
        return null;
    }
  }

  public Dictionary<string, int> GetTeamsCount()
  {
    Dictionary<string, int> teamsCount = new()
    {
      { "traveler", 0 },
      { "townsfolk", 0 },
      { "outsider", 0 },
      { "minion", 0 },
      { "demon", 1 }
    };
    int playerCount = gamestate.players.Length;

    int travellersCount = Mathf.Max(playerCount - 15, 0);
    int travellersRealCount = 0;
    foreach(var role in gamestate.players)
    {
      if(!rolesData.ContainsKey(role.role)) continue;
      if(rolesData[role.role].team == "traveler") travellersRealCount++;
    }
    teamsCount["traveler"] = Mathf.Max(travellersCount, travellersRealCount);
    playerCount = Mathf.Max(playerCount - teamsCount["traveler"], 0);

    int outsiderCount = (playerCount + 2) % 3;
    if(playerCount < 7) outsiderCount -= 1;
    if(playerCount > 15) outsiderCount = 2;
    teamsCount["outsider"] = Mathf.Clamp(outsiderCount, 0, 4);
    teamsCount["minion"] = Mathf.Clamp((playerCount - 1) / 3 - 1, 1, 3);
    teamsCount["demon"] = 1;
    teamsCount["townsfolk"] = playerCount -
      teamsCount["outsider"] - teamsCount["minion"] - teamsCount["demon"];

    return teamsCount;
  }

  public Dictionary<string, List<(string, int)>> GetTeamsCountMod()
  {
    Dictionary<string, List<(string, int)>> teamsMods = new();

    foreach(var role in gamestate.players)
    {
      if(!rolesData.ContainsKey(role.role)) continue;

      var modifiers = rolesData[role.role].teamCountModifiers;
      if(modifiers == null) continue;

      foreach(var modifier in modifiers)
      {
        if(!teamsMods.ContainsKey(modifier.team))
        {
          teamsMods[modifier.team] = new List<(string, int)>();
        }

        teamsMods[modifier.team].Add((modifier.action, modifier.countModifier));
      }
    }
    foreach(var role in gamestate.fabled)
    {
      if(!fabledData.ContainsKey(role.id)) continue;

      var modifiers = fabledData[role.id].teamCountModifiers;
      if(modifiers == null) continue;

      foreach(var modifier in modifiers)
      {
        if(!teamsMods.ContainsKey(modifier.team))
        {
          teamsMods[modifier.team] = new List<(string, int)>();
        }

        teamsMods[modifier.team].Add((modifier.action, modifier.countModifier));
      }
    }

    return teamsMods;
  }


  public EditionData? GetEditionData(string id)
  {
    return editionsData.ContainsKey(id) ? editionsData[id] : null;
  }
  public string GetEditionName(string id)
  {
    if(id == "custom") return "Custom";
    if(!editionsData.ContainsKey(id)) return null;
    return editionsData[id].name;
  }
  public Texture2D GetEditionImg(string id)
  {
    var edition = GetEditionData(id);
    if(edition != null || id == "custom")
    {
      return Resources.Load<Texture2D>("Images/editions/" + id);
    }

    return null;
  }


  public Dictionary<string, List<string>> SeparateByTeam(List<string> roleIDs)
  {
    Dictionary<string, List<string>> r = new();

    foreach(string id in roleIDs)
    {
      if(!rolesData.ContainsKey(id) && !fabledData.ContainsKey(id))
      {
        if(!r.ContainsKey("unknown"))
        {
          r.Add("unknown", new());
        }
        r["unknown"].Add(id);
        continue;
      }

      string team = rolesData.ContainsKey(id) ? rolesData[id].team : fabledData[id].team;
      if(!r.ContainsKey(team))
      {
        r.Add(team, new());
      }
      r[team].Add(id);
    }

    return r;
  }

  public List<string> OrderByName(List<string> roleIDs)
  {
    return roleIDs.OrderBy(id =>
    {
      string rename = TokensManager.Instance.GetRename(id);
      if(rename != null) return rename;

      if(rolesData.ContainsKey(id))
      {
        return rolesData[id].name;
      }
      else if(fabledData.ContainsKey(id))
      {
        return fabledData[id].name;
      }
      return id;
    }).ToList();
  }

  public List<string> OrderByNight(List<string> roleIDs)
  {
    return roleIDs.OrderBy(id =>
    {
      if(rolesData.ContainsKey(id))
      {
        return rolesData[id].firstNight > 0 ? rolesData[id].firstNight : int.MaxValue;
      }
      else if(fabledData.ContainsKey(id))
      {
        return fabledData[id].firstNightReminder != "" ? 1 : int.MaxValue;
      }
      return int.MaxValue;
    }).ThenBy(id =>
    {
      if(rolesData.ContainsKey(id))
      {
        return rolesData[id].otherNight > 0 ? rolesData[id].otherNight : int.MaxValue;
      }
      else if(fabledData.ContainsKey(id))
      {
        return fabledData[id].otherNight > 0 ? fabledData[id].otherNight : int.MaxValue;
      }
      return int.MaxValue;
    }).ToList();
  }

  public List<string> OrderByTeam(List<string> roleIDs)
  {
    return roleIDs.OrderBy(id =>
    {
      if(rolesData.ContainsKey(id))
      {
        switch(rolesData[id].team)
        {
          case "traveler":
            return 0;
          case "townsfolk":
            return 1;
          case "outsider":
            return 2;
          case "minion":
            return 3;
          case "demon":
            return 4;
          default:
            return 5;
        }
      }
      else if(fabledData.ContainsKey(id))
      {
        return 6;
      }
      return int.MaxValue;
    }).ToList();
  }

  public List<ReminderInfo> OrderReminders(List<ReminderInfo> reminders)
  {
    return reminders
      .OrderBy(r =>
      {
        if(rolesData.ContainsKey(r.role))
        {
          switch(rolesData[r.role].team)
          {
            case "traveler":
              return 0;
            case "townsfolk":
              return 1;
            case "outsider":
              return 2;
            case "minion":
              return 3;
            case "demon":
              return 4;
            default:
              return 5;
          }
        }
        else if(fabledData.ContainsKey(r.role))
        {
          return 6;
        }
        return int.MaxValue;
      })
      .ThenBy(r =>
      {
        if(rolesData.ContainsKey(r.role))
        {
          return rolesData[r.role].firstNight > 0 ? rolesData[r.role].firstNight : int.MaxValue;
        }
        else if(fabledData.ContainsKey(r.role))
        {
          return fabledData[r.role].firstNightReminder != "" ? 1 : int.MaxValue;
        }
        return int.MaxValue;
      })
      .ThenBy(r =>
      {
        if(rolesData.ContainsKey(r.role))
        {
          return rolesData[r.role].otherNight > 0 ? rolesData[r.role].otherNight : int.MaxValue;
        }
        else if(fabledData.ContainsKey(r.role))
        {
          return fabledData[r.role].otherNight > 0 ? fabledData[r.role].otherNight : int.MaxValue;
        }
        return int.MaxValue;
      })
      .ThenBy(r => r.role)
      .ThenBy(r => r.name)
      .ToList();
  }

  public Dictionary<string, List<string>> OrderByNightAndSeparateByTeam(List<string> roleIDs)
  {
    roleIDs = OrderByNight(roleIDs);
    return SeparateByTeam(roleIDs);
  }

  public Dictionary<string, List<string>> OrderByNameAndSeparateByTeam(List<string> roleIDs)
  {
    roleIDs = OrderByName(roleIDs);
    return SeparateByTeam(roleIDs);
  }


  public int GetFirstNightInPlayOrder(string id)
  {
    if(!rolesData.ContainsKey(id)) return 0;
    int order =  rolesData[id].firstNight;
    if (order == 0) return 0;

    int r = 1;
    List<int> repeatedOrders = new();

    foreach (var p in gamestate.players)
    {
      if (!rolesData.ContainsKey(p.role) ||
          rolesData[p.role].firstNight == 0 ||
          repeatedOrders.Contains(rolesData[p.role].firstNight)) continue;
      r += rolesData[p.role].firstNight < order ? 1 : 0;
      repeatedOrders.Add(rolesData[p.role].firstNight);
    }
    foreach(var f in gamestate.fabled)
    {
      if(!fabledData.ContainsKey(f.id) || fabledData[f.id].firstNightReminder == "") continue;
      if (repeatedOrders.Contains(1)) continue;
      r += 1 < order ? 1 : 0;
      repeatedOrders.Add(1);
    }

    return r;
  }
  public int GetOtherNightInPlayOrder(string id)
  {
    if(!rolesData.ContainsKey(id)) return 0;
    int order = rolesData[id].otherNight;
    if(order == 0) return 0;

    int r = 1;
    List<int> repeatedOrders = new();

    foreach(var p in gamestate.players)
    {
      if(!rolesData.ContainsKey(p.role) ||
         rolesData[p.role].otherNight == 0 ||
         repeatedOrders.Contains(rolesData[p.role].otherNight)) continue;
      r += rolesData[p.role].otherNight < order ? 1 : 0;
      repeatedOrders.Add(rolesData[p.role].otherNight);
    }
    foreach(var f in gamestate.fabled)
    {
      if(!fabledData.ContainsKey(f.id) ||
         fabledData[f.id].otherNight == 0 ||
         repeatedOrders.Contains(fabledData[f.id].otherNight)) continue;
      r += fabledData[f.id].otherNight < order ? 1 : 0;
      repeatedOrders.Add(fabledData[f.id].otherNight);
    }

    return r;
  }
  public int GetOtherNightInPlayOrderFabled(string id)
  {
    if(!fabledData.ContainsKey(id)) return 0;
    int order = fabledData[id].otherNight;
    if(order == 0) return 0;

    int r = 1;
    List<int> repeatedOrders = new();

    foreach(var p in gamestate.players)
    {
      if(!rolesData.ContainsKey(p.role) ||
         rolesData[p.role].otherNight == 0 ||
         repeatedOrders.Contains(rolesData[p.role].otherNight)) continue;
      r += rolesData[p.role].otherNight < order ? 1 : 0;
      repeatedOrders.Add(rolesData[p.role].otherNight);
    }
    foreach(var f in gamestate.fabled)
    {
      if(!fabledData.ContainsKey(f.id) ||
         fabledData[f.id].otherNight == 0 ||
         repeatedOrders.Contains(fabledData[f.id].otherNight)) continue;
      r += fabledData[f.id].otherNight < order ? 1 : 0;
      repeatedOrders.Add(fabledData[f.id].otherNight);
    }

    return r;
  }



  public void StartAddingPlayer()
  {
    AskForPlayerName("Choose a name for the new player:", (name) =>
    {
      AddPlayer(name);
    });
  }

  public void AddPlayer(string name)
  {
    PlayerInfo newPlayer = new PlayerInfo();
    newPlayer.name = name;
    newPlayer.role = "";
    newPlayer.reminders = new List<ReminderInfo>().ToArray();
    newPlayer.isDead = false;
    newPlayer.isVoteless = false;

    List<PlayerInfo> playersList = gamestate.players.ToList();
    playersList.Add(newPlayer);
    gamestate.players = playersList.ToArray();
    SaveGameState();
    UpdatePlayerCount();

    tokenRingObj.AddPlayerToken(newPlayer);
  }


  public void UpdatePlayerCount()
  {
    gameStateObj.SetPlayersCount(
      gamestate.players.Length,
      gamestate.players.Where(p => p.isDead).Count(),
      gamestate.players.Where(p => !p.isVoteless).Count(),
      GetTeamsCount(),
      GetTeamsCountMod()
    );
  }
}
