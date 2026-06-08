using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SyntheticDataSetGenerator : MonoBehaviour
{
    [Header("Objects to capture")]
    public List<GameObject> objectsToCapture;

    [Header("Camera settings")]
    public Camera captureCamera;
    public Vector3 cameraOffset = new Vector3(0, 1, -2);

    [Header("Capture settings")]
    public int imagesPerObject = 20;       // Kuinka monta kuvaa per objekti
    public string outputFolder = "SyntheticData";

    void Start()
    {
        if (!Directory.Exists(outputFolder))
            Directory.CreateDirectory(outputFolder);

        StartCoroutine(CaptureAllObjects());
    }

    IEnumerator CaptureAllObjects()
    {
        foreach (GameObject obj in objectsToCapture)
        {
            string classFolder = Path.Combine(outputFolder, obj.name);
            if (!Directory.Exists(classFolder))
                Directory.CreateDirectory(classFolder);

            for (int i = 0; i < imagesPerObject; i++)
            {
                // Sijoita kamera satunnaiseen kulmaan
                Vector3 randomOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    Random.Range(0.5f, 1.5f),
                    Random.Range(-1f, -2f)
                );
                captureCamera.transform.position = obj.transform.position + cameraOffset + randomOffset;
                captureCamera.transform.LookAt(obj.transform);

                yield return new WaitForEndOfFrame(); // Varmistaa että renderointi on valmis

                Texture2D tex = TakeScreenshot();
                string path = Path.Combine(classFolder, $"{obj.name}_{i}.png");
                SaveTextureToPNG(tex, path);

                yield return null; // Antaa seuraavan ruudun päivityksen
            }
        }

        Debug.Log("Dataset generation complete!");
    }

    private Texture2D TakeScreenshot()
    {
        RenderTexture rt = new RenderTexture(224, 224, 24);
        captureCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(224, 224, TextureFormat.RGB24, false);
        captureCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 224, 224), 0, 0);
        screenShot.Apply();
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        return screenShot;
    }

    private void SaveTextureToPNG(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        Debug.Log("Saved: " + path);
    }
}
