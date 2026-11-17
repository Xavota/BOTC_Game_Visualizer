using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class CircularText : MonoBehaviour
{
  public float radius = 100f;

  private TMP_Text text;
  public string Text
  {
    set
    {
      text ??= GetComponent<TMP_Text>();
      text.text = value;
    }
  }

  private void Awake()
  {
    text = GetComponent<TMP_Text>();
  }


  void LateUpdate()
  {
    text ??= GetComponent<TMP_Text>();
    text.ForceMeshUpdate();

    TMP_TextInfo textInfo = text.textInfo;

    int count = textInfo.characterCount;
    if(count == 0)
      return;

    // Width from the beginning of the first character
    // to the end of the last character.
    float textStart = textInfo.characterInfo[0].origin;
    float textEnd =
        textInfo.characterInfo[textInfo.characterCount - 1].xAdvance;

    float totalWidth = textEnd - textStart;

    // We offset by half the width so the complete text is centered.
    float textCenter = textStart + totalWidth * 0.5f;

    for(int i = 0; i < count; i++)
    {
      TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

      // Spaces do not have vertices, but their xAdvance still affects
      // the positions of the characters after them.
      if(!charInfo.isVisible)
        continue;

      int materialIndex = charInfo.materialReferenceIndex;
      int vertexIndex = charInfo.vertexIndex;

      Vector3[] vertices =
          textInfo.meshInfo[materialIndex].vertices;

      // The horizontal center of this character's allotted space.
      float characterCenter =
          (charInfo.origin + charInfo.xAdvance) * 0.5f;

      // Signed distance from the center of the complete text.
      float distanceFromTextCenter =
          characterCenter - textCenter;

      // Arc length formula: angle = distance / radius.
      float angleRadians =
          -distanceFromTextCenter / radius;
      angleRadians += Mathf.PI; // Offset by 90 degrees so the text is upright.

      float angleDegrees =
          angleRadians * Mathf.Rad2Deg;

      Vector3 baselineCenter = new Vector3(
          characterCenter,
          charInfo.baseLine,
          0f
      );

      Quaternion rotation =
          Quaternion.Euler(0f, 0f, -angleDegrees);

      Vector3 targetPosition = new Vector3(
          Mathf.Sin(angleRadians) * radius,
          Mathf.Cos(angleRadians) * radius,
          0f
      );

      Matrix4x4 matrix =
          Matrix4x4.TRS(
              targetPosition,
              rotation,
              new Vector3(-1f, -1f, 1f)
          )
          * Matrix4x4.Translate(-baselineCenter);

      for(int j = 0; j < 4; j++)
      {
        vertices[vertexIndex + j] =
            matrix.MultiplyPoint3x4(
                vertices[vertexIndex + j]
            );
      }
    }

    for(int i = 0; i < textInfo.meshInfo.Length; i++)
    {
      TMP_MeshInfo meshInfo = textInfo.meshInfo[i];

      meshInfo.mesh.vertices = meshInfo.vertices;
      text.UpdateGeometry(meshInfo.mesh, i);
    }
  }
}