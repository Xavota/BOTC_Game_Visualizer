using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RolesPopup : MonoBehaviour
{
  [SerializeField] private TMP_Text titleText;
  [SerializeField] private VBox contentColumn;
  [SerializeField] private VBox rolesBox;
  private List<GameObject> tokens = new();
  private List<HBox> rows = new();

  public event TokensManager.IDDelegate OnTokenSelected;
  public event TokensManager.VoidDelegate OnClosed;

  public int minColumnsCount = 4;
  public int maxColumnsCount = 10;
  public float aspectRatio = 3.0f;

  public void Init(List<string> roleIDs, string title = null)
  {
    if (title != null && titleText != null)
    {
      titleText.text = title;
    }

    ClearTokens();

    int rolesCount = roleIDs.Count;

    if (aspectRatio <= 0.0f) aspectRatio = 2.0f;

    float squareCount = rolesCount / aspectRatio;
    int rowsCount = Mathf.RoundToInt(Mathf.Sqrt(squareCount));

    if(rowsCount <= 0) rowsCount = 1;

    int maxRolesPerRow = Mathf.Clamp(
      Mathf.CeilToInt((float)rolesCount / rowsCount),
      minColumnsCount, maxColumnsCount
    );

    for (int i = 0; i < rolesCount; i += maxRolesPerRow)
    {
      List<string> rowRoleIDs = roleIDs.GetRange(i, Mathf.Min(maxRolesPerRow, rolesCount - i));
      CreateNewRow(rowRoleIDs);
    }
  }

  public void ClearTokens()
  {
    foreach (var t in tokens)
    {
      Destroy(t);
    }
    tokens.Clear();

    foreach(var t in rows)
    {
      Destroy(t.gameObject);
    }
    rows.Clear();
  }

  private void CreateNewRow(List<string> roles)
  {
    if (roles == null || roles.Count == 0) return;

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
    hBoxComp.OnFinishedUpdate += (o, _) =>
    {
      HBox box = (HBox)o;
      if(box == null) return;
      box.enabled = false;
    };
    rows.Add(hBoxComp);

    adjustToChildrenComp.adjustHeight = true;

    newRowObj.transform.SetParent(rolesBox.transform);
    ((RectTransform)newRowObj.transform).anchoredPosition = new Vector2(0.0f, 0.0f);

    TokensManager tManager = TokensManager.Instance;
    if(tManager.basicRoleTokenPrefab == null) return;

    int rowCount = roles.Count;
    for (int i = 0; i < rowCount; ++i)
    {
      var role = roles[i];
      BasicRoleToken newToken = Instantiate(tManager.basicRoleTokenPrefab, newRowObj.transform);
      ((RectTransform)newToken.transform).anchoredPosition = new Vector2(0.0f, 0.0f);
      ((RectTransform)newToken.transform).sizeDelta = new Vector2(130.0f, 130.0f);
      newToken.Init(role);
      newToken.SetHaloColor(tManager.GetRoleColor(role));
      newToken.OnLeftClick += (o, _) => OnTokenSelected.Invoke(((BasicRoleToken)o).role);
      newToken.SetAbilityReminderSide(i * 2 >= rowCount);
      tokens.Add(newToken.gameObject);
    }
  }

  public void Close()
  {
    OnClosed?.Invoke();
  }
}