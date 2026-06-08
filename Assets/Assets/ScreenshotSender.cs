using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class ScreenshotSender : MonoBehaviour
{
    [SerializeField] private string serverUrl = "https://22bc2e901150.ngrok-free.app";
    public void SendScreenshot(Texture2D texture)
    {
        StartCoroutine(UploadScreenshot(texture));
    }
    [System.Serializable]
    class ImagePayload { public string img; }
    private IEnumerator UploadScreenshot(Texture2D texture)
    {
        byte[] png = texture.EncodeToPNG();
        string b64 = System.Convert.ToBase64String(png);
        var payload = new ImagePayload { img = b64 };
        string json = JsonUtility.ToJson(payload);

        using (var www = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(body);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError(www.error);
            else
                Debug.Log("Server response: " + www.downloadHandler.text);
        }
    }
}
