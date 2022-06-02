using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zilliqa.Requests;

namespace Zilliqa.Core
{
    public class ZilliqaRPC
    {
        public static string apiUrl = "https://dev-api.zilliqa.com/";//"https://api.zilliqa.com/";
        public static async Task<GetTxBlockResponse> GetLatestTxBlock()
        {
            ZilRequest getTxBlockReq = new ZilRequest("GetLatestTxBlock", new object[] { "" });
            //Response is identical to GetTxBlock
            var result = await PostRequest<GetTxBlockResponse>(getTxBlockReq);

            return result;
        }

        public static async Task<GetTxBlockRateResponse> GetTxBlockRate()
        {
            ZilRequest getTxBlockReq = new ZilRequest("GetTxBlockRate", new object[] { "" });
            //Response is identical to GetTxBlock
            var result = await PostRequest<GetTxBlockRateResponse>(getTxBlockReq);

            return result;
        }

        protected static async Task<T> PostRequest<T>(ZilRequest request, Action<T, ZilResponse.Error> onComplete = null)
       where T : ZilResponse
        {
            string json = request.ToJson();
            //Debug.Log(json);
            using UnityEngine.Networking.UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST");
            byte[] rawData = Encoding.UTF8.GetBytes(json);

            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.uploadHandler = new UploadHandlerRaw(rawData);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            await webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    var response = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);
                    onComplete?.Invoke(response, response.error);
                    return response;
                default:
                    return null;
            }
        }
    }
}