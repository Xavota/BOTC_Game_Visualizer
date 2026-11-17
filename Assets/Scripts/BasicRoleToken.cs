using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicRoleToken : MonoBehaviour
{
  public event EventHandler OnLeftClick;
  public event EventHandler OnMouseEnter;
  public event EventHandler OnMouseExit;

  [HideInInspector] public string role;
  [HideInInspector] public RoleData? roleData;
  [HideInInspector] public FabledData? fabledData;

  [SerializeField] private RawImage roleImg;
  [SerializeField] private CircularText circularRoleText;
  [SerializeField] private TMP_Text plainRoleText;

  [SerializeField] private GameObject leaf1;
  [SerializeField] private GameObject leaf2;
  [SerializeField] private GameObject leaf3;
  [SerializeField] private GameObject leaf4;
  [SerializeField] private GameObject leaf5;
  [SerializeField] private GameObject leafLeft;
  [SerializeField] private GameObject leafRight;
  [SerializeField] private GameObject leafOrange;

  [SerializeField] private NightOrder firstNightOrderObj;
  [SerializeField] private NightOrder otherNightsOrderObj;

  [SerializeField] private GameObject firstNightOrderReminderObj;
  [SerializeField] private TMP_Text firstNightOrderReminderText;
  [SerializeField] private GameObject otherNightsOrderReminderObj;
  [SerializeField] private TMP_Text otherNightsOrderReminderText;

  [SerializeField] private GameObject abilityReminder;
  [SerializeField] private TMP_Text abilityReminderText;

  [SerializeField] private GameObject removeIcon;
  [HideInInspector] public bool canBeRemoved;

  [SerializeField] private RawImage halo;


  [SerializeField] private UIMouseDetector mouseDetector;

  [SerializeField] private ChangeSiblingOrder siblingOrder;
  [HideInInspector] public bool shouldChangeSiblingOrder = true;


  public bool IsActive
  {
    set
    {
      if(firstNightOrderObj != null) firstNightOrderObj.IsActive = value;
      if(otherNightsOrderObj != null) otherNightsOrderObj.IsActive = value;
    }
  }


  public void Init(string role, bool? isActive = null, bool canBeRemoved = false, bool shouldChangeSiblingOrder = true)
  {
    this.role = role;
    this.canBeRemoved = canBeRemoved;
    this.shouldChangeSiblingOrder = shouldChangeSiblingOrder;
    if(isActive != null) IsActive = isActive.Value;

    SetVisibleInfo();
    SubscribeToPointerEvents();
  }

  public void ChangeRole(string id)
  {
    role = id;
    SetVisibleInfo();
  }

  private void Update()
  {
    AdjustCircularTextRadius();
  }

  private void AdjustCircularTextRadius()
  {
    if(circularRoleText == null) return;
    RectTransform t = (RectTransform)transform;
    float innersSize = Mathf.Min(t.rect.width, t.rect.height) - 10.0f;
    circularRoleText.radius = innersSize * 0.425f;
  }


  private void HideRoleInfo()
  {
    if(roleImg != null) roleImg.gameObject.SetActive(false);
    if(circularRoleText != null) circularRoleText.gameObject.SetActive(false);
    if(plainRoleText != null) plainRoleText.gameObject.SetActive(false);
  }

  private void HideLeafs()
  {
    if(leaf1 != null) leaf1.SetActive(false);
    if(leaf2 != null) leaf2.SetActive(false);
    if(leaf3 != null) leaf3.SetActive(false);
    if(leaf4 != null) leaf4.SetActive(false);
    if(leaf5 != null) leaf5.SetActive(false);
    if(leafLeft != null) leafLeft.SetActive(false);
    if(leafRight != null) leafRight.SetActive(false);
    if(leafOrange != null) leafOrange.SetActive(false);
  }

  private void HideNightsOrder()
  {
    if(firstNightOrderObj != null) firstNightOrderObj.gameObject.SetActive(false);
    if(otherNightsOrderObj != null) otherNightsOrderObj.gameObject.SetActive(false);
  }

  private void HideComponents()
  {
    HideRoleInfo();
    HideLeafs();
    HideNightsOrder();
  }


  private void SetVisibleInfo()
  {
    HideComponents();

    if(abilityReminderText != null) abilityReminderText.text = "";

    if(role == "" || role == null)
    {
      this.roleData = null;
      this.fabledData = null;
      return;
    }

    var tManager = TokensManager.Instance;


    RoleData? roleData = tManager.GetRoleData(role);
    if(roleData != null)
    {
      SetRoleData(roleData.Value);
      return;
    }


    FabledData? fabledData = tManager.GetFabledData(role);
    if(fabledData != null)
    {
      SetFabledData(fabledData.Value);
      return;
    }

    SetCenterCharacterName(role);
  }

  public void SetRoleData(RoleData roleData)
  {
    fabledData = null;
    this.roleData = roleData;

    var tManager = TokensManager.Instance;

    var img = tManager.GetRoleImage(role);

    string rename = TokensManager.Instance.GetRename(roleData.id);
    SetRoleInfo(rename?? roleData.name, img);

    SetLeafsIndicators(
      roleData.reminders.Length +
      roleData.remindersGlobal.Length,
      roleData.setup,
      roleData.firstNight != 0,
      roleData.otherNight != 0
    );

    int firstNight = TokensManager.Instance.GetFirstNightInPlayOrder(roleData.id);
    int otherNights = TokensManager.Instance.GetOtherNightInPlayOrder(roleData.id);
    SetNightOrders(
      firstNight,
      roleData.firstNightReminder,
      otherNights,
      roleData.otherNightReminder
    );

    if(abilityReminderText != null) abilityReminderText.text = roleData.ability;
  }

  public void SetFabledData(FabledData fabledData)
  {
    roleData = null;
    this.fabledData = fabledData;

    var tManager = TokensManager.Instance;

    var img = tManager.GetFabledImage(role);

    string rename = TokensManager.Instance.GetRename(fabledData.id);
    SetRoleInfo(rename?? fabledData.name, img);

    SetLeafsIndicators(
      fabledData.reminders.Length,
      fabledData.setup,
      fabledData.firstNightReminder != "" &&
      fabledData.firstNightReminder != null,
      fabledData.otherNight != 0
    );

    int otherNights = TokensManager.Instance.GetOtherNightInPlayOrderFabled(fabledData.id);
    SetNightOrders(
      fabledData.firstNightReminder != "" ? 1 : 0,
      fabledData.firstNightReminder,
      otherNights,
      fabledData.otherNightReminder
    );

    if(abilityReminderText != null) abilityReminderText.text = fabledData.ability;
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


  private void SetLeafsIndicators(int remindersCount, bool specialCharacter,
                                  bool firstNight, bool otherNights)
  {
    HideLeafs();

    switch(remindersCount)
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

    if(leafLeft != null) leafLeft.SetActive(firstNight);
    if(leafRight != null) leafRight.SetActive(otherNights);

    if(leafOrange == null) return;
    leafOrange.SetActive(specialCharacter);
  }


  public void ReloadNightOrderInfo()
  {
    if(roleData != null)
    {
      int firstNight = TokensManager.Instance.GetFirstNightInPlayOrder(roleData.Value.id);
      int otherNights = TokensManager.Instance.GetOtherNightInPlayOrder(roleData.Value.id);
      SetNightOrders(
        firstNight,
        roleData.Value.firstNightReminder,
        otherNights,
        roleData.Value.otherNightReminder
      );
    }
    else if(fabledData != null)
    {
      int otherNights = TokensManager.Instance.GetOtherNightInPlayOrderFabled(fabledData.Value.id);
      SetNightOrders(
        fabledData.Value.firstNightReminder != "" ? 1 : 0,
        fabledData.Value.firstNightReminder,
        otherNights,
        fabledData.Value.otherNightReminder
      );
    }
  }

  private void SetNightOrders(int firstNight, string firstNightReminder,
                              int otherNights, string otherNightsReminder,
                              bool? isActive = null)
  {
    if(firstNightOrderObj != null)
    {
      if(firstNightOrderReminderText != null) firstNightOrderReminderText.text = firstNightReminder ?? "";
      if(isActive != null) firstNightOrderObj.IsActive = isActive.Value;
      firstNightOrderObj.gameObject.SetActive(firstNight != 0);
      if(firstNight != 0)
      {
        firstNightOrderObj.Order = firstNight;
      }
    }

    if(otherNightsOrderObj != null)
    {
      if(otherNightsOrderReminderText != null) otherNightsOrderReminderText.text = otherNightsReminder ?? "";
      if(isActive != null) otherNightsOrderObj.IsActive = isActive.Value;
      otherNightsOrderObj.gameObject.SetActive(otherNights != 0);
      if(otherNights == 0) return;

      otherNightsOrderObj.Order = otherNights;
    }
  }


  public void SetHaloColor(Color? color)
  {
    if(halo == null) return;
    halo.color = color ?? Color.clear;
    halo.gameObject.SetActive(color != null);
  }


  private void SubscribeToPointerEvents()
  {
    if(mouseDetector != null)
    {
      mouseDetector.OnEnter -= OnDetectorEnter;
      mouseDetector.OnExit -= OnDetectorExit;
      mouseDetector.OnLeftClick -= OnDetectorLeftClick;
      mouseDetector.OnExit -= OnDetectorMouseExit;
      mouseDetector.OnEnter -= OnDetectorMouseEnter;

      mouseDetector.OnEnter += OnDetectorEnter;
      mouseDetector.OnExit += OnDetectorExit;
      mouseDetector.OnLeftClick += OnDetectorLeftClick;
      mouseDetector.OnExit += OnDetectorMouseExit;
      mouseDetector.OnEnter += OnDetectorMouseEnter;
    }


    if (firstNightOrderObj != null && firstNightOrderObj.mouseDetector != null)
    {
      firstNightOrderObj.mouseDetector.OnEnter -= OnFirstNightOrderEnter;
      firstNightOrderObj.mouseDetector.OnExit -= OnFirstNightOrderExit;

      firstNightOrderObj.mouseDetector.OnEnter += OnFirstNightOrderEnter;
      firstNightOrderObj.mouseDetector.OnExit += OnFirstNightOrderExit;
    }
    if(otherNightsOrderObj != null && otherNightsOrderObj.mouseDetector != null)
    {
      otherNightsOrderObj.mouseDetector.OnEnter -= OnOtherNightOrderEnter;
      otherNightsOrderObj.mouseDetector.OnExit -= OnOtherNightOrderExit;

      otherNightsOrderObj.mouseDetector.OnEnter += OnOtherNightOrderEnter;
      otherNightsOrderObj.mouseDetector.OnExit += OnOtherNightOrderExit;
    }
  }

  private void OnDetectorEnter(object o, System.EventArgs e)
  {
    if(removeIcon != null) removeIcon.SetActive(canBeRemoved);
  }
  private void OnDetectorExit(object o, System.EventArgs e)
  {
    if(removeIcon != null) removeIcon.SetActive(false);
  }
  private void OnDetectorLeftClick(object o, System.EventArgs e)
  {
    OnLeftClick?.Invoke(this, EventArgs.Empty);
  }
  private void OnDetectorMouseExit(object o, System.EventArgs e)
  {
    OnMouseExit?.Invoke(this, EventArgs.Empty);
    if(abilityReminder != null && (abilityReminderText?.text?? "") != "") abilityReminder.SetActive(false);
    if(shouldChangeSiblingOrder && siblingOrder != null) siblingOrder.ReturnToOriginal();
  }
  private void OnDetectorMouseEnter(object o, System.EventArgs e)
  {
    OnMouseEnter?.Invoke(this, EventArgs.Empty);
    if(abilityReminder != null && (abilityReminderText?.text ?? "") != "") abilityReminder.SetActive(true);
    if(shouldChangeSiblingOrder && siblingOrder != null) siblingOrder.SetAsLast();
  }

  private void OnFirstNightOrderEnter(object o, System.EventArgs e)
  {
    OnMouseEnter?.Invoke(this, EventArgs.Empty);
    if (firstNightOrderReminderObj != null) firstNightOrderReminderObj.SetActive(true);
    if(shouldChangeSiblingOrder && siblingOrder != null) siblingOrder.SetAsLast();
  }
  private void OnFirstNightOrderExit(object o, System.EventArgs e)
  {
    OnMouseExit?.Invoke(this, EventArgs.Empty);
    if(firstNightOrderReminderObj != null) firstNightOrderReminderObj.SetActive(false);
    if(shouldChangeSiblingOrder && siblingOrder != null) siblingOrder.ReturnToOriginal();
  }
  private void OnOtherNightOrderEnter(object o, System.EventArgs e)
  {
    OnMouseEnter?.Invoke(this, EventArgs.Empty);
    if(otherNightsOrderReminderObj != null) otherNightsOrderReminderObj.SetActive(true);
    if(shouldChangeSiblingOrder && siblingOrder != null) siblingOrder.SetAsLast();
  }
  private void OnOtherNightOrderExit(object o, System.EventArgs e)
  {
    OnMouseExit?.Invoke(this, EventArgs.Empty);
    if(otherNightsOrderReminderObj != null) otherNightsOrderReminderObj.SetActive(false);
    if(shouldChangeSiblingOrder && siblingOrder != null) siblingOrder.ReturnToOriginal();
  }

  public void SetNightOrderVisibility(bool state)
  {
    int firstNight = 0;
    int otherNights = 0;
    if (roleData != null)
    {
      firstNight = TokensManager.Instance.GetFirstNightInPlayOrder(roleData.Value.id);
      otherNights = TokensManager.Instance.GetOtherNightInPlayOrder(roleData.Value.id);
    }
    else if(fabledData != null)
    {
      otherNights = TokensManager.Instance.GetOtherNightInPlayOrderFabled(fabledData.Value.id);
    }
    firstNightOrderObj.gameObject.SetActive(firstNight != 0 && state);
    otherNightsOrderObj.gameObject.SetActive(otherNights != 0 && state);
  }

  public void SetAbilityReminderVisibility(bool state)
  {
    if(abilityReminder != null) abilityReminder.SetActive(state);
  }


  public void SetAbilityReminderSide(bool left)
  {
    if(abilityReminder == null) return;
    RectTransform t = (RectTransform)abilityReminder.transform;
    if(t == null) return;
    t.anchoredPosition = new Vector2(left ? -25.0f : 25.0f, 0.0f);
    t.pivot = new Vector2(left ? 1.0f : 0.0f, 0.5f);
    t.anchorMin = new Vector2(left ? 0.0f : 1.0f, 0.5f);
    t.anchorMax = new Vector2(left ? 0.0f : 1.0f, 0.5f);
  }
}
