using System.IO;
using UnityEngine;

public class ReferenceSheetExporter : MonoBehaviour
{
  [SerializeField] private Camera renderCamera;
  [SerializeField] private RenderTexture renderTexture;

  public void ExportSheet(string fileName)
  {
    // Store previous active RT
    RenderTexture previous = RenderTexture.active;

    // Render camera into the render texture
    renderCamera.targetTexture = renderTexture;
    renderCamera.Render();

    // Read pixels from RenderTexture into Texture2D
    RenderTexture.active = renderTexture;
    Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
    image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
    image.Apply();

    // Clean up render targets
    renderCamera.targetTexture = null;
    RenderTexture.active = previous;

    // Encode to PNG
    byte[] pngData = image.EncodeToPNG();

    // Save file
    string filePath = Path.Combine(Application.persistentDataPath, fileName + ".png");
    File.WriteAllBytes(filePath, pngData);

    Debug.Log("Saved sheet to: " + filePath);

    // Optional cleanup
    Destroy(image);
  }
}