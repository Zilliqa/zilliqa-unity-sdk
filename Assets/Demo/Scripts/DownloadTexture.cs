using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MetadataUtils
{

    public static async Task LoadTextureFromWeb(string imageURL, Action<Sprite> onComplete)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL);
        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + www.error);
            return;
        }

        Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
        var downloadedImage = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);

        onComplete?.Invoke(downloadedImage);
    }
}
