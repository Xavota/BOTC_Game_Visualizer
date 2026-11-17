using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Token : MonoBehaviour
{
  [HideInInspector] public PlayerInfo info;

  [SerializeField] private GameObject dmInfoParent;
  [SerializeField] private GameObject hiddenInfoParent;

  [SerializeField] private RawImage roleImg;
  [SerializeField] private CircularText circularRoleText;
  [SerializeField] private TMP_Text plainRoleText;
  [SerializeField] private NightOrder firstNightOrderObj;
  [SerializeField] private NightOrder otherNightsOrderObj;
  [SerializeField] private GameObject visibleDeathShroud;
  [SerializeField] private GameObject hiddenAliveIndicator;
  [SerializeField] private GameObject visibleGhostVote;
  [SerializeField] private GameObject hiddenGhostVote;

  [SerializeField] private GameObject leaf1;
  [SerializeField] private GameObject leaf2;
  [SerializeField] private GameObject leaf3;
  [SerializeField] private GameObject leaf4;
  [SerializeField] private GameObject leaf5;
  [SerializeField] private GameObject leafOrange;

  [SerializeField] private TMP_Text nameText;

  [SerializeField] private GameObject remindersParentObj;
  private readonly List<Reminder> reminderObjs = new();

  [SerializeField] private float reminderSpacing = 85.0f;
  [SerializeField] private float reminderInitialDistance = 150.0f;

  private float tokenAngle = 0.0f;

  public void Init(PlayerInfo info, float tokenAngle)
  {
    this.info = info;
    this.tokenAngle = tokenAngle;

    SetVisibleInfo();
    CreateReminders(info.reminders.ToList());
  }


  private void HideRoleInfo()
  {
    if(roleImg != null) roleImg.gameObject.SetActive(false);
    if(circularRoleText != null) circularRoleText.gameObject.SetActive(false);
    if(plainRoleText != null) plainRoleText.gameObject.SetActive(false);
  }

  private void HideNightsOrder()
  {
    if(firstNightOrderObj != null) firstNightOrderObj.gameObject.SetActive(false);
    if(otherNightsOrderObj != null) otherNightsOrderObj.gameObject.SetActive(false);
  }

  private void HideDeathInfo()
  {
    if(visibleDeathShroud != null) visibleDeathShroud.SetActive(false);
    if(hiddenAliveIndicator != null) hiddenAliveIndicator.SetActive(false);
    if(visibleGhostVote != null) visibleGhostVote.SetActive(false);
    if(hiddenGhostVote != null) hiddenGhostVote.SetActive(false);
  }

  private void HideLeafs()
  {
    if(leaf1 != null) leaf1.SetActive(false);
    if(leaf2 != null) leaf2.SetActive(false);
    if(leaf3 != null) leaf3.SetActive(false);
    if(leaf4 != null) leaf4.SetActive(false);
    if(leaf5 != null) leaf5.SetActive(false);
    if(leafOrange != null) leafOrange.SetActive(false);
  }

  private void HideReminders()
  {
    if(remindersParentObj != null) remindersParentObj.SetActive(false);
  }

  private void HideComponents()
  {
    HideRoleInfo();
    HideNightsOrder();
    HideDeathInfo();
    HideLeafs();
    HideReminders();
  }


  private void SetVisibleInfo()
  {
    HideComponents();


    nameText.text = info.name;
    SetDeathAndVoteVisibility(info.isDead, info.isVoteless);

    if (info.role == "" || info.role == null) return;


    var tManager = TokensManager.Instance;

    RoleData? characterData = tManager.GetRoleData(info.role);

    if (characterData == null)
    {
      SetCenterCharacterName(info.role);
      return;
    }


    var img = tManager.GetRoleImage(info.role);

    SetRoleInfo(characterData.Value.name, img);

    SetNightOrders(
      characterData.Value.firstNight,
      characterData.Value.otherNight
    );

    SetLeafsIndicators(
      characterData.Value.reminders.Length +
      characterData.Value.remindersGlobal.Length,
      characterData.Value.setup
    );
  }


  private void SetRoleInfo(string name, Texture2D img)
  {
    if(roleImg != null && img != null)
    {
      SetCharacterImage(img);
      SetCircularCharacterName(name);
    }
    else
    {
      SetCenterCharacterName(name);
    }
  }

  private void SetCenterCharacterName(string name)
  {
    if(plainRoleText == null) return;

    plainRoleText.text = name ?? "";
    plainRoleText.gameObject.SetActive(name != null);
  }

  private void SetCircularCharacterName(string name)
  {
    if(circularRoleText == null) return;

    circularRoleText.Text = name ?? "";
    circularRoleText.gameObject.SetActive(name != null);
  }

  private void SetCharacterImage(Texture2D img)
  {
    if(roleImg == null) return;

    roleImg.texture = img;
    roleImg.gameObject.SetActive(img != null);
  }


  private void SetNightOrders(int firstNight, int otherNights)
  {
    if(firstNightOrderObj != null)
    {
      firstNightOrderObj.gameObject.SetActive(firstNight != 0);
      if(firstNight != 0)
      {
        firstNightOrderObj.Order = firstNight;
      }
    }

    if(otherNightsOrderObj != null)
    {
      otherNightsOrderObj.gameObject.SetActive(otherNights != 0);
      if(otherNights == 0) return;

      otherNightsOrderObj.Order = otherNights;
    }
  }


  private void SetLeafsIndicators(int remindersCount, bool specialCharacter)
  {
    HideLeafs();

    switch (remindersCount)
    {
      case 1:
        if(leaf1 != null) leaf1.SetActive(true);
        break;
      case 2:
        if(leaf2 != null) leaf2.SetActive(true);
        break;
      case 3:
        if(leaf3 != null) leaf3.SetActive(true);
        break;
      case 4:
        if(leaf4 != null) leaf4.SetActive(true);
        break;
      case 5:
        if(leaf5 != null) leaf5.SetActive(true);
        break;
      default:
        break;
    }

    if(leafOrange == null) return;
    leafOrange.SetActive(specialCharacter);
  }


  private void SetDeathAndVoteVisibility(bool isDead, bool isVoteless)
  {
    if(visibleDeathShroud != null)
    {
      visibleDeathShroud.SetActive(isDead);
    }
    if(hiddenAliveIndicator != null)
    {
      hiddenAliveIndicator.SetActive(!isDead);
    }

    if(visibleGhostVote != null)
    {
      visibleGhostVote.SetActive(isDead && !isVoteless);
    }
    if(hiddenGhostVote != null)
    {
      hiddenGhostVote.SetActive(isDead && !isVoteless);
    }
  }


  private void CreateReminders(List<ReminderInfo> reminders)
  {
    var tManager = TokensManager.Instance;

    Reminder reminderPrefab = tManager.reminderPrefab;
    if(reminderPrefab == null)
    {
      reminderPrefab = Resources.Load<Reminder>("Reminder");
      if(reminderPrefab == null) return;
    }

    reminderObjs.Clear();

    int reminderCount = reminders.Count;
    if(reminderCount <= 0) return;

    if(remindersParentObj != null) remindersParentObj.SetActive(true);


    Vector3 remindersDirection =
      new(-Mathf.Cos(tokenAngle), Mathf.Sin(tokenAngle), 0.0f);

    remindersParentObj.transform.position =
      transform.position + remindersDirection * reminderInitialDistance;

    for(int i = 0; i < reminderCount; i++)
    {
      Vector3 reminderPosition =
        remindersParentObj.transform.position +
        remindersDirection * reminderSpacing * i;

      Reminder rem = Instantiate(
        reminderPrefab.gameObject,
        reminderPosition,
        Quaternion.identity,
        remindersParentObj.transform
      ).GetComponent<Reminder>();

      if (rem != null)
      {
        rem.Init(reminders[i]);
        reminderObjs.Add(rem);
      }
    }
  }
}
