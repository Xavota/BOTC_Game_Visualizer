using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerToken : MonoBehaviour
{
  public delegate void PlayerActionDelegate(PlayerToken playerObj);
  public delegate void StatusChangeDelegate(PlayerToken playerObj, bool status);
  public delegate void RoleChangeDelegate(PlayerToken playerObj, string role);
  public delegate bool ReminderBoolReturnDelegate(PlayerToken playerObj, int reminderIndex);

  [HideInInspector] public PlayerInfo info;

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


  [SerializeField] private BasicRoleToken nightOrderToken;
  [SerializeField] private UIMouseDetector hiddenInfoMouseDetector;
  [SerializeField] private UIMouseDetector nameMouseDetector;

  [SerializeField] private GameObject visibleInfoParent;
  [SerializeField] private GameObject hiddenInfoParent;

  [SerializeField] private DeadShroud deadShroud;
  [SerializeField] private GameObject hiddenAliveIndicator;
  [SerializeField] private DeadVote visibleGhostVote;
  [SerializeField] private GameObject hiddenGhostVote;

  [SerializeField] private TMP_Text nameText;

  [SerializeField] private GameObject remindersParentObj;
  private List<Reminder> reminderObjs = new();
  [SerializeField] private Reminder plusReminder;

  [SerializeField] private float reminderSpacing = 85.0f;
  [SerializeField] private float reminderInitialDistance = 150.0f;

  [HideInInspector] public float tokenAngle = 0.0f;

  [SerializeField] private ChangeSiblingOrder siblingOrder;
  [SerializeField] private PlayerActionsMenu actionsMenu;

  [SerializeField] private MultipleImageBtn groupActionBtn;


  public void Init(PlayerInfo info, float tokenAngle)
  {
    this.info = info;
    this.tokenAngle = tokenAngle;

    if(nightOrderToken != null)
    {
      nightOrderToken.OnLeftClick += (o, _) =>
      {
        TokensManager.Instance.AskForRole(
          "Select a role for " + info.name + ":",
          (role) =>
          {
            this.info.role = role;
            nightOrderToken.ChangeRole(role);
            OnRoleChange?.Invoke(this, role);
          }
        );
      };
      nightOrderToken.OnMouseEnter += (o, _) => OnPointerEnterToken();
      nightOrderToken.OnMouseExit += (o, _) => OnPointerExitToken();
      nightOrderToken.IsActive = !info.isDead;
      nightOrderToken.SetAbilityReminderSide(Mathf.Cos(this.tokenAngle) > 0.0f);
      nightOrderToken.Init(info.role, null, false, false);
    }
    SetVisibleInfo();
    CreateReminders(info.reminders.ToList());

    if(deadShroud != null)
    {
      deadShroud.Init(info.isDead);
      deadShroud.OnLeftClick += (o, _) =>
      {
        SetDeadState(!this.info.isDead);
      };
      deadShroud.OnEnter += (o, _) =>
      {
        if(nightOrderToken != null) nightOrderToken.SetAbilityReminderVisibility(true);
        OnPointerEnterToken();
      };
      deadShroud.OnExit += (o, _) =>
      {
        if(nightOrderToken != null) nightOrderToken.SetAbilityReminderVisibility(false);
        OnPointerExitToken();
      };
    }
    if(visibleGhostVote != null)
    {
      visibleGhostVote.Init(info.isVoteless);
      visibleGhostVote.OnLeftClick += (o, _) =>
      {
        SetVotelessState(!this.info.isVoteless);
      };
    }

    if(hiddenInfoMouseDetector != null)
    {
      hiddenInfoMouseDetector.OnLeftClick += (o, _) =>
      {
        if(!this.info.isDead)
        {
          SetDeadState(true);
        }
        else if(!this.info.isVoteless)
        {
          SetVotelessState(true);
        }
        else
        {
          SetDeadState(false);
        }
      };
    }

    if(nameMouseDetector != null)
    {
      nameMouseDetector.OnEnter += (o, _) =>
      {
        OnPointerEnterToken();
      };
      nameMouseDetector.OnExit += (o, _) =>
      {
        OnPointerExitToken();
      };
      nameMouseDetector.OnLeftClick += (o, _) =>
      {
        SetActionsMenuActive(!actionsMenu.gameObject.activeSelf);
      };
    }

    if(actionsMenu != null)
    {
      actionsMenu.Init();
      actionsMenu.OnEnter += (o, _) =>
      {
        OnPointerEnterToken();
      };
      actionsMenu.OnExit += (o, _) =>
      {
        OnPointerExitToken();
      };
      actionsMenu.OnRename += () =>
      {
        OnRename?.Invoke(this);
      };
      actionsMenu.OnMove += () =>
      {
        OnMove?.Invoke(this);
      };
      actionsMenu.OnSwap += () =>
      {
        OnSwap?.Invoke(this);
      };
      actionsMenu.OnRemove += () =>
      {
        OnRemove?.Invoke(this);
      };
      actionsMenu.OnNominate += () =>
      {
        OnStartNomination?.Invoke(this);
      };
      /*actionsMenu.OnNominate += () =>
      {
        OnNominate?.Invoke(this);
      };*/
      SetActionsMenuSide(Mathf.Cos(this.tokenAngle) > 0.0f);
    }
  }


  private void OnPointerEnterToken(bool showReminder = true)
  {
    if(plusReminder != null) plusReminder.Show();
    if(siblingOrder != null) siblingOrder.SetAsLast();
  }
  private void OnPointerExitToken(bool showReminder = true)
  {
    if(plusReminder != null) plusReminder.Hide();
    if(siblingOrder != null) siblingOrder.ReturnToOriginal();
  }


  public void SetDeadState(bool state)
  {
    info.isDead = state;
    info.isVoteless = false;
    nightOrderToken.IsActive = !state;
    OnDeadStatusChange?.Invoke(this, state);
    SetDeathAndVoteVisibility(state, info.isVoteless);
  }

  public void SetVotelessState(bool state)
  {
    info.isVoteless = state;
    OnVotelessStatusChange?.Invoke(this, state);
    SetDeathAndVoteVisibility(info.isDead, state);
  }


  private void SetVisibleInfo()
  {
    nameText.text = info.name;
    SetDeathAndVoteVisibility(info.isDead, info.isVoteless);
  }

  private void SetDeathAndVoteVisibility(bool isDead, bool isVoteless)
  {
    if(deadShroud != null) deadShroud.SetState(isDead);
    if(hiddenAliveIndicator != null) hiddenAliveIndicator.SetActive(!isDead);

    if(visibleGhostVote != null)
    {
      visibleGhostVote.gameObject.SetActive(isDead);
      visibleGhostVote.SetState(isVoteless);
    }
    if(hiddenGhostVote != null)
    {
      hiddenGhostVote.SetActive(isDead && !isVoteless);
    }
  }


  private void CreateReminders(List<ReminderInfo> reminders)
  {
    foreach(var rem in reminderObjs)
    {
      if(rem != null) Destroy(rem.gameObject);
    }
    reminderObjs.Clear();

    if(remindersParentObj == null) return;

    int reminderCount = reminders.Count;

    var tManager = TokensManager.Instance;

    Reminder reminderPrefab = tManager.reminderPrefab;
    if(reminderPrefab == null)
    {
      reminderPrefab = Resources.Load<Reminder>("Reminder");
      if(reminderPrefab == null) return;
    }

    for(int i = 0; i < reminderCount; i++)
    {
      Reminder rem = Instantiate(
        reminderPrefab.gameObject,
        remindersParentObj.transform
      ).GetComponent<Reminder>();
      rem.canBeRemoved = true;
      rem.OnLeftClick += OnReminderLeftClick;
      rem.OnMouseEnter += (o, _) => OnPointerEnterToken(false);
      rem.OnMouseExit += (o, _) => OnPointerExitToken(false);

      if(rem != null)
      {
        rem.Init(reminders[i]);
        reminderObjs.Add(rem);
      }
    }

    if(plusReminder != null)
    {
      plusReminder.canBeRemoved = false;
      plusReminder.Init(new ReminderInfo("plus", ""));
      plusReminder.OnLeftClick += (o, _) =>
      {
        OnReminderAdd?.Invoke(this);
      };
      plusReminder.OnMouseEnter += (o, _) => OnPointerEnterToken(false);
      plusReminder.OnMouseExit += (o, _) => OnPointerExitToken(false);
      plusReminder.Hide();
    }

    ReloadReminders();
  }


  private void OnReminderLeftClick(object o, System.EventArgs e)
  {
    var reminderObj = (o as Reminder);
    if(reminderObj == null) return;
    if(OnReminderRemove?.Invoke(this, reminderObjs.IndexOf(reminderObj)) ?? false)
    {
      Destroy(reminderObj.gameObject);
      reminderObjs.Remove(reminderObj);
      List<ReminderInfo> reminders = new List<ReminderInfo>(info.reminders);
      reminders.Remove(reminderObj.info);
      info.reminders = reminders.ToArray();

      ReloadReminders();
    }
  }


  public void ChangeRole(string newRole)
  {
    info.role = newRole;
    if(nightOrderToken != null) nightOrderToken.ChangeRole(newRole);
  }


  public void AddReminder(ReminderInfo remInfo)
  {
    var tManager = TokensManager.Instance;

    Reminder reminderPrefab = tManager.reminderPrefab;
    if(reminderPrefab == null)
    {
      reminderPrefab = Resources.Load<Reminder>("Reminder");
      if(reminderPrefab == null) return;
    }

    Reminder rem = Instantiate(
      reminderPrefab.gameObject,
      remindersParentObj.transform
    ).GetComponent<Reminder>();
    rem.canBeRemoved = true;
    rem.OnLeftClick += OnReminderLeftClick;

    if(rem != null)
    {
      rem.Init(remInfo);
      reminderObjs.Add(rem);
    }

    ReloadReminders();
  }

  public void ReloadReminders()
  {
    int reminderCount = reminderObjs.Count;

    Vector3 remindersDirection =
      new(-Mathf.Cos(tokenAngle), Mathf.Sin(tokenAngle), 0.0f);

    remindersParentObj.transform.position =
      transform.position + remindersDirection * reminderInitialDistance;

    Vector3 reminderPosition = new();
    for(int i = 0; i < reminderCount; i++)
    {
      reminderPosition =
        remindersParentObj.transform.position +
        remindersDirection * reminderSpacing * i;

      reminderObjs[i].transform.position = reminderPosition;
    }

    if(plusReminder == null) return;

    reminderPosition =
      remindersParentObj.transform.position +
      remindersDirection * reminderSpacing * reminderCount;

    plusReminder.transform.position = reminderPosition;

    SetActionsMenuSide(Mathf.Cos(this.tokenAngle) > 0.0f);
    if(nightOrderToken != null)
      nightOrderToken.SetAbilityReminderSide(Mathf.Cos(this.tokenAngle) > 0.0f);
  }

  public void RemoveAllReminders()
  {
    info.reminders = (new List<ReminderInfo>()).ToArray();
    foreach (var r in reminderObjs)
    {
      Destroy(r.gameObject);
    }
    reminderObjs.Clear();
  }


  public void ReloadNightOrderInfo()
  {
    if(nightOrderToken != null) nightOrderToken.ReloadNightOrderInfo();
  }


  public void SetVisibilityState(bool state)
  {
    visibleInfoParent.SetActive(state);
    hiddenInfoParent.SetActive(!state);
  }

  public void SetNightOrderVisibility(bool state)
  {
    nightOrderToken.SetNightOrderVisibility(state);
  }


  public void SetActionsMenuSide(bool left)
  {
    if(actionsMenu == null) return;
    RectTransform t = (RectTransform)actionsMenu.transform;
    if(t == null) return;
    t.anchoredPosition = new Vector2(left ? -25.0f : 25.0f, 0.0f);
    t.pivot = new Vector2(left ? 1.0f : 0.0f, 0.0f);
    t.anchorMin = new Vector2(left ? 0.0f : 1.0f, 0.0f);
    t.anchorMax = new Vector2(left ? 0.0f : 1.0f, 0.0f);
  }

  public void SetActionsMenuActive(bool active)
  {
    if(actionsMenu != null) actionsMenu.gameObject.SetActive(active);
    if(nameText != null) nameText.color = active ? Color.red : Color.white;
  }


  public void Rename(string newName)
  {
    if(nameText != null) nameText.text = newName;
  }

  public void AskForGroupAction(Texture2D img, PlayerActionDelegate callback)
  {
    if(groupActionBtn == null) return;

    groupActionBtn.Image = img;
    groupActionBtn.OnPressed.AddListener(() => callback(this));
    groupActionBtn.gameObject.SetActive(true);
  }

  public void HideGroupAction()
  {
    if(groupActionBtn == null) return;

    groupActionBtn.OnPressed.RemoveAllListeners();
    groupActionBtn.Reset();
    groupActionBtn.gameObject.SetActive(false);
  }
}
