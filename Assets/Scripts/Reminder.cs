using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Reminder : MonoBehaviour
{
  public event EventHandler OnLeftClick;
  public event EventHandler OnMouseEnter;
  public event EventHandler OnMouseExit;

  [SerializeField] private UIMouseDetector mouseDetector;

  [HideInInspector] public ReminderInfo info;

  [SerializeField] private RawImage image;
  [SerializeField] private TMP_Text centerNameText;
  [SerializeField] private TMP_Text bottomNameText;

  [SerializeField] private GameObject removeIcon;
  [HideInInspector] public bool canBeRemoved;


  [SerializeField] private GameObject tokenObj;
  [SerializeField] private GameObject tokenBackground;

  public void Init(ReminderInfo info)
  {
    this.info = info;

    SubscribeToPointerEvents();

    var tManager = TokensManager.Instance;

    if(!tManager.ReminderExist(info.role, info.name))
    {
      SetImage(null);
      SetCenterName(info.name);
      return;
    }

    var img = tManager.GetReminderImage(info.role, info.name);

    if(img == null || image == null)
    {
      SetImage(null);
      SetCenterName(info.name);
      return;
    }

    SetImage(img);
    SetBottomName(info.name);
  }

  private void SetCenterName(string name)
  {
    if(centerNameText == null) return;

    centerNameText.text = name?? "";
    centerNameText.gameObject.SetActive(name != null);

    if(name == null) return;

    SetBottomName(null);
  }

  private void SetBottomName(string name)
  {
    if(bottomNameText == null) return;

    bottomNameText.text = name ?? "";
    bottomNameText.gameObject.SetActive(name != null);

    if(name == null) return;

    SetCenterName(null);
  }

  private void SetImage(Texture2D img)
  {
    if(image == null) return;

    image.texture = img;
    image.gameObject.SetActive(img != null);
  }


  private void SubscribeToPointerEvents()
  {
    if(mouseDetector == null) return;

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
  }
  private void OnDetectorMouseEnter(object o, System.EventArgs e)
  {
    OnMouseEnter?.Invoke(this, EventArgs.Empty);
  }


  public void Show()
  {
    if(tokenObj != null) tokenObj.SetActive(true);
    if(tokenBackground != null) tokenBackground.SetActive(true);
  }

  public void Hide()
  {
    if(tokenObj != null) tokenObj.SetActive(false);
    if(tokenBackground != null) tokenBackground.SetActive(false);
  }
}