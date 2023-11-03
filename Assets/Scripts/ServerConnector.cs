using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ServerConnector : MonoBehaviour
{
    [SerializeField]
    private Text debugLog;

    private const string SERVER_URL = "http://localhost:3000/retrieve-string";

    void Start()
    {
        StartCoroutine(GetStringFromServer());
    }

    IEnumerator GetStringFromServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(SERVER_URL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                debugLog.text = "Error: " + www.error;
            }
            else
            {
                debugLog.text = www.downloadHandler.text; // Output: Hello from Node.js server!
            }
        }
    }
}