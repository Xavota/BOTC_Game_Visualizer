using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RemindersPopup : MonoBehaviour
{
  [SerializeField] private TMP_Text titleText;
  [SerializeField] private VBox contentColumn;
  [SerializeField] private VBox rolesBox;
  private List<GameObject> tokensAndRows = new();

  public event TokensManager.ReminderDelegate OnTokenSelected;
  public event TokensManager.VoidDelegate OnClosed;

  public int minColumnsCount = 5;
  public int maxColumnsCount = 11;
  public float aspectRatio = 3.0f;

  public void Init(List<ReminderInfo> reminders, string title = null)
  {
    if(title != null && titleText != null)
    {
      titleText.text = title;
    }

    ClearTokens();

    int rolesCount = reminders.Count;

    if(aspectRatio <= 0.0f) aspectRatio = 2.0f;

    float squareCount = (float)rolesCount / aspectRatio;
    int rowsCount = Mathf.RoundToInt(Mathf.Sqrt(squareCount));

    if(rowsCount <= 0) rowsCount = 1;

    int maxRolesPerRow = Mathf.Clamp(
      Mathf.CeilToInt((float)rolesCount / rowsCount),
      minColumnsCount, maxColumnsCount
    );

    for(int i = 0; i < rolesCount; i += maxRolesPerRow)
    {
      List<ReminderInfo> rowRoleIDs = reminders.GetRange(i, Mathf.Min(maxRolesPerRow, rolesCount - i));
      CreateNewRow(rowRoleIDs);
    }
  }

  public void ClearTokens()
  {
    foreach(var t in tokensAndRows)
    {
      Destroy(t);
    }
    tokensAndRows.Clear();
  }

  private void CreateNewRow(List<ReminderInfo> roles)
  {
    if(roles == null || roles.Count == 0) return;

    if(rolesBox == null)
    {
      GameObject newColumnObj = new GameObject();
      rolesBox = newColumnObj.AddComponent<VBox>();
      AdjustToChildren adjustToChildrenCompWidth =
        newColumnObj.AddComponent<AdjustToChildren>();

      rolesBox.spacing = 35;
      rolesBox.adjustWidthToFitChildren = true;

      adjustToChildrenCompWidth.adjustWidth = true;

      rolesBox.transform.parent = contentColumn.transform;
      ((RectTransform)newColumnObj.transform).anchoredPosition = new Vector2(0.0f, 0.0f);
    }

    GameObject newRowObj = new GameObject();
    HBox hBoxComp = newRowObj.AddComponent<HBox>();
    AdjustToChildren adjustToChildrenComp =
      newRowObj.AddComponent<AdjustToChildren>();

    hBoxComp.spacing = 35;
    hBoxComp.adjustWidthToFitChildren = true;
    hBoxComp.alignment = HBoxAlignment.Center;
    tokensAndRows.Add(hBoxComp.gameObject);

    adjustToChildrenComp.adjustHeight = true;

    newRowObj.transform.SetParent(rolesBox.transform);
    ((RectTransform)newRowObj.transform).anchoredPosition = new Vector2(0.0f, 0.0f);

    TokensManager tManager = TokensManager.Instance;
    if(tManager.basicRoleTokenPrefab == null) return;

    foreach(ReminderInfo role in roles)
    {
      Reminder newToken = Instantiate(tManager.reminderPrefab, newRowObj.transform);
      ((RectTransform)newToken.transform).anchoredPosition = new Vector2(0.0f, 0.0f);
      ((RectTransform)newToken.transform).sizeDelta = new Vector2(90.0f, 90.0f);
      newToken.Init(role);
      newToken.OnLeftClick += (o, _) => OnTokenSelected.Invoke(((Reminder)o).info);
      tokensAndRows.Add(newToken.gameObject);
    }
  }

  public void Close()
  {
    OnClosed?.Invoke();
  }
}