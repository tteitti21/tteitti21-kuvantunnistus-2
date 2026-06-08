using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private Texture2D TakePhoto()
    {
        RenderTexture rt = new RenderTexture(640, 480, 23);
        Camera.main.targetTexture = rt;
        Texture2D screenShot = new Texture2D(640, 480, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, 640, 480), 0, 0);
        Camera.main.targetTexture = null;
        Destroy(rt);

        return screenShot;
    }

    private void SaveTextureToPNG(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);
        Debug.Log(path);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Take ScreenShot
            Texture2D texture = TakePhoto();

            // Save ScreenShot
            SaveTextureToPNG(texture, Application.dataPath + "/1.png");

            // NEW: Send Screenshot to Server
            ScreenshotSender sender = FindAnyObjectByType<ScreenshotSender>();
            if (sender != null)
            {
                sender.SendScreenshot(texture);
            }
            else
            {
                Debug.LogWarning("No ScreenshotSender found in scene!");
            }
        }
    }
}
