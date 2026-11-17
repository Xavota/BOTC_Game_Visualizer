using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeSiblingOrder : MonoBehaviour
{
  private int originalSiblingIndex;

  public void SetAsLast()
  {
    originalSiblingIndex = transform.GetSiblingIndex();
    transform.SetAsLastSibling();
  }

  public void ReturnToOriginal()
  {
    transform.SetSiblingIndex(originalSiblingIndex);
  }
}
