using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ZilliqaMonoBehaviour : MonoBehaviour
{
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    protected IEnumerator PostRequest<T>(ZilRequest request, Action<T, ZilResponse.Error> onComplete = null)
       where T : ZilResponse
    {
        string json = request.ToJson();
        using UnityEngine.Networking.UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST");
        byte[] rawData = Encoding.UTF8.GetBytes(json);

        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.uploadHandler = new UploadHandlerRaw(rawData);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                var response = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                onComplete?.Invoke(response, response.error);
                break;
            default:
                break;
        }
    }
}
