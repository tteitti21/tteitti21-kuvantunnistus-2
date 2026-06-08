using UnityEngine;
using System.Collections;
using System.IO;

public class CaptureObjectImages : MonoBehaviour
{
    public Camera sceneCapture;
    public Transform targetObject;
    public int horizontalSteps = 32; // how many around horizontally
    public int verticalSteps = 30;    // how many heights
    public float verticalAngleRange = 45f; // max tilt above/below

    void Start()
    {
        StartCoroutine(CaptureImages());
    }

    IEnumerator CaptureImages()
    {
        // Ensure the object has a Renderer
        Renderer rend = targetObject.GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogError("Target object has no Renderer attached!");
            yield break;
        }

        Bounds bounds = rend.bounds;
        float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float distance = size * 2.0f;
        Vector3 objectCenter = bounds.center;

        int captureIndex = 0;

        // --- Create root + object subfolder ---
        string rootDir = Path.Combine(Application.dataPath, "SyntheticData");
        string objectDir = Path.Combine(rootDir, targetObject.name);
        if (!Directory.Exists(objectDir))
            Directory.CreateDirectory(objectDir);

        // Loop vertical angles
        for (int v = 0; v < verticalSteps; v++)
        {
            float vNorm = (verticalSteps > 1) ? (float)v / (verticalSteps - 1) : 0.5f;
            float verticalAngle = Mathf.Lerp(-verticalAngleRange, verticalAngleRange, vNorm);

            // Loop horizontal angles
            for (int h = 0; h < horizontalSteps; h++)
            {
                float horizontalAngle = h * (360f / horizontalSteps);

                Quaternion rotation = Quaternion.Euler(verticalAngle, horizontalAngle, 0);
                Vector3 offset = rotation * new Vector3(0, 0, -distance);
                sceneCapture.transform.position = objectCenter + offset;
                sceneCapture.transform.LookAt(objectCenter);

                yield return new WaitForEndOfFrame();

                // Capture
                RenderTexture rt = new RenderTexture(224, 224, 24);
                sceneCapture.targetTexture = rt;
                Texture2D tex = new Texture2D(224, 224, TextureFormat.RGB24, false);
                sceneCapture.Render();
                RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, 224, 224), 0, 0);
                tex.Apply();
                sceneCapture.targetTexture = null;
                RenderTexture.active = null;
                Destroy(rt);

                // Save to object-specific folder
                string filePath = Path.Combine(objectDir, $"capture_{captureIndex}.png");
                File.WriteAllBytes(filePath, tex.EncodeToPNG());
                Debug.Log("Saved: " + filePath);

                captureIndex++;
            }
        }
    }
}
