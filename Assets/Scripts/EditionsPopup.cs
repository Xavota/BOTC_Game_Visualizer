using System.Collections.Generic;
using UnityEngine;

public class EditionsPopup : MonoBehaviour
{
  [SerializeField] private VBox contentColumn;
  [SerializeField] private VBox editionsBox;
  private List<GameObject> btns = new();
  private List<HBox> rows = new();

  public event TokensManager.IDDelegate OnEditionSelected;
  public event TokensManager.VoidDelegate OnClosed;

  public int minColumnsCount = 2;
  public int maxColumnsCount = 4;
  public float aspectRatio = 2.0f;

  public void Init(List<string> editionIDs)
  {
    ClearBtns();

    int editionsCount = editionIDs.Count;

    if(aspectRatio <= 0.0f) aspectRatio = 2.0f;

    float squareCount = editionsCount / aspectRatio;
    int rowsCount = Mathf.RoundToInt(Mathf.Sqrt(squareCount));

    if(rowsCount <= 0) rowsCount = 1;

    int maxEditionsPerRow = Mathf.Clamp(
      Mathf.CeilToInt((float)editionsCount / rowsCount),
      minColumnsCount, maxColumnsCount
    );

    for(int i = 0; i < editionsCount; i += maxEditionsPerRow)
    {
      List<string> rowEditionIDs = editionIDs.GetRange(i, Mathf.Min(maxEditionsPerRow, editionsCount - i));
      CreateNewRow(rowEditionIDs);
    }
  }

  public void ClearBtns()
  {
    foreach(var t in btns)
    {
      Destroy(t);
    }
    btns.Clear();

    foreach(var t in rows)
    {
      Destroy(t.gameObject);
    }
    rows.Clear();
  }

  private void CreateNewRow(List<string> editions)
  {
    if(editions == null || editions.Count == 0) return;

    if(editionsBox == null)
    {
      GameObject newColumnObj = new GameObject();
      editionsBox = newColumnObj.AddComponent<VBox>();
      AdjustToChildren adjustToChildrenCompWidth =
        newColumnObj.AddComponent<AdjustToChildren>();

      editionsBox.spacing = 15;
      editionsBox.adjustWidthToFitChildren = true;

      adjustToChildrenCompWidth.adjustWidth = true;
      adjustToChildrenCompWidth.minSize = new Vector2(650, 0);

      editionsBox.transform.parent = contentColumn.transform;
      ((RectTransform)newColumnObj.transform).anchoredPosition = new Vector2(0.0f, 0.0f);
    }

    GameObject newRowObj = new GameObject();
    HBox hBoxComp = newRowObj.AddComponent<HBox>();
    AdjustToChildren adjustToChildrenComp =
      newRowObj.AddComponent<AdjustToChildren>();

    hBoxComp.spacing = 10;
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

    newRowObj.transform.SetParent(editionsBox.transform);
    ((RectTransform)newRowObj.transform).anchoredPosition = new Vector2(0.0f, 0.0f);

    TokensManager tManager = TokensManager.Instance;
    if(tManager.editionBtnPrefab == null) return;

    int rowCount = editions.Count;
    for(int i = 0; i < rowCount; ++i)
    {
      var edition = editions[i];
      EditionBtn newBtn = Instantiate(tManager.editionBtnPrefab, newRowObj.transform);
      ((RectTransform)newBtn.transform).anchoredPosition = new Vector2(0.0f, 0.0f);
      ((RectTransform)newBtn.transform).sizeDelta = new Vector2(350.0f, 215.0f);
      newBtn.Init(edition);
      newBtn.OnLeftClick += (o, _) => OnEditionSelected.Invoke(((EditionBtn)o).edition);
      btns.Add(newBtn.gameObject);
    }
  }

  public void Close()
  {
    OnClosed?.Invoke();
  }
}