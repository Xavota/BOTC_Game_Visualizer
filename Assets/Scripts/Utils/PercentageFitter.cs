using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(RectTransform))]
public class PercentageFitter : MonoBehaviour
{
  public Vector2 topLeft = new();
  public Vector2 size = new();

  void Start()
  {
    GetComponent<RectTransform>().anchorMin = topLeft;
    GetComponent<RectTransform>().anchorMax = topLeft + size;
  }

  void Update()
  {

  }
}