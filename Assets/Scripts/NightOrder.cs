using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NightOrder : MonoBehaviour
{
  [SerializeField] private TMP_Text numberText;
  [SerializeField] private RawImage gradient;
  [SerializeField] private Color baseGradientColor;

  public UIMouseDetector mouseDetector;

  public int Order
  {
    set
    {
      if(numberText != null) numberText.text = value.ToString();
    }
  }

  public bool IsActive
  {
    set
    {
      if(gradient != null) gradient.color = value ? baseGradientColor : Color.gray;
    }
  }
}