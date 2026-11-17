using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditionBtn : MonoBehaviour
{
  public event System.EventHandler OnLeftClick;

  public string edition;

  [SerializeField] private UIMouseDetector mouseDetector;
  [SerializeField] private TMP_Text nameText;
  [SerializeField] private RawImage image;


  public void Init(string id)
  {
    edition = id;
    TokensManager tManager = TokensManager.Instance;
    var editionName = tManager.GetEditionName(id);
    if(editionName == null) return;

    if(nameText != null) nameText.text = editionName;

    var editionImg = tManager.GetEditionImg(id);
    if(image != null && editionImg != null && editionImg.width > 0)
    {
      image.texture = editionImg;
      RectTransform t = (RectTransform)image.transform;
      float invAspect = (float)editionImg.height / editionImg.width;
      t.sizeDelta = new Vector2(t.sizeDelta.x, t.sizeDelta.x * invAspect);
    }

    if (mouseDetector != null)
    {
      mouseDetector.OnLeftClick += (o, _) => OnLeftClick?.Invoke(this, _);
    }
  }
}