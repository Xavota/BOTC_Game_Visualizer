using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MultipleImageBtn : MonoBehaviour
{
  private bool justSelected = false;
  private bool selected = false;

  [SerializeField] private List<RawImage> colorImages;
  [SerializeField] private List<RawImage> staticImages;
  [SerializeField] private Texture2D texture;
  public Texture2D Image
  {
    set
    {
      ChangeImage(value);
    }
  }
  [SerializeField] private Color normalColor = Color.white;
  [SerializeField] private Color hoveredColor = Color.gray8;
  [SerializeField] private Color pressedColor = Color.gray5;
  [SerializeField] private Color selectedColor = Color.coral;

  [SerializeField] private UIMouseDetector mouseDetector;

  public UnityEvent OnPressed;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Awake()
  {
    ChangeImage(texture);
    ChangeColor(normalColor);

    if(mouseDetector == null) return;

    mouseDetector.OnEnter += (o, _) =>
    {
      if(selected) return;
      ChangeColor(hoveredColor);
    };
    mouseDetector.OnExit += (o, _) =>
    {
      if(selected) return;
      ChangeColor(normalColor);
    };
    mouseDetector.OnLeftClick += (o, _) =>
    {
      OnPressed?.Invoke();

      if(selected) return;
      justSelected = true;
      ChangeColor(pressedColor);
    };
    mouseDetector.OnClickOutside += (o, _) =>
    {
      selected = false;
      ChangeColor(normalColor);
    };
  }

  void Update()
  {
    if (justSelected)
    {
      justSelected = false;
      selected = true;
      ChangeColor(selectedColor);
    }
  }

  public void ChangeColor(Color newColor)
  {
    foreach(RawImage img in colorImages)
    {
      if(img == null) continue;
      img.color = newColor;
    }
  }

  public void ChangeImage(Texture2D newImage)
  {
    texture = newImage;
    foreach(RawImage img in colorImages)
    {
      if(img == null) continue;
      img.texture = newImage;
    }
    foreach(RawImage img in staticImages)
    {
      if(img == null) continue;
      img.texture = newImage;
    }
  }

  public void Reset()
  {
    mouseDetector.ResetStateMachine();
    selected = false;
    ChangeColor(normalColor);
  }
}