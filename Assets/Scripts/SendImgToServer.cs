using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class SendImgToServer : MonoBehaviour
{
    [SerializeField]
    private Text debugLog;

    [SerializeField]
    private Text jobLog;

    private byte[] screenshotBytes;

    public void OnButtonClicked()
    {
        debugLog.text += "Button Clicked from Direct Method!";
        StartCoroutine(CaptureImage());
    }
    private IEnumerator CaptureImage()
    {
        debugLog.text += "Button Clicked!";

        yield return new WaitForEndOfFrame();

        jobLog.text = "Capturing Image... ";

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        debugLog.text += $"Captured Texture Dimensions: {screenshot.width}x{screenshot.height}";

        //StartCoroutine(AddImageJob(texture));
        //texture to bytes
        screenshotBytes = screenshot.EncodeToPNG();
        debugLog.text = "";
        debugLog.text += "Starting Upload... ";
        StartCoroutine(UploadScreenshot(screenshotBytes));
    }

    IEnumerator UploadScreenshot(byte[] screenshotData)
    {
        string uploadURL = "http://localhost:3000/upload";
        UnityWebRequest www = new UnityWebRequest(uploadURL, "POST");
        www.uploadHandler = new UploadHandlerRaw(screenshotData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/octet-stream");
        debugLog.text += "Server Response: " + www.downloadHandler.text;

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            debugLog.text += www.error;
        }
        else
        {
            debugLog.text += "Screenshot uploaded successfully!";
        }
    }
}
