using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-num-txns-txepoch
 */
public class GetNumTxnsTxEpoch : MonoBehaviour
{
    const string METHOD = "GetNumTxnsTxEpoch";
    
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    public bool showDebug = true;

    [Serializable]
    struct GetNumTxnsTxEpochRequest
    {
        public int id;
        public string jsonrpc;
        public string method;
        public List<string> paramsList;
    }

    void Start()
    {
        try
        {
            GetNumTxnsTxEpochRequest getNumTxnsTxEpoch = new GetNumTxnsTxEpochRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = METHOD,
                paramsList = new List<string>()
            };

            string json = JsonUtility.ToJson(getNumTxnsTxEpoch);
            json = json.Replace("paramsList", "params");

            if (showDebug)
                Debug.Log(METHOD + ":\n" + json);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            
            byte[] pData = System.Text.Encoding.ASCII.GetBytes(json.ToCharArray());

            WWW api = new WWW(apiUrl, pData, headers);

            StartCoroutine(Utils.WaitForWWW(api, showDebug, METHOD));
        }
        catch (UnityException ex)
        {
            Debug.Log(ex.Message);
        }
    }
}
