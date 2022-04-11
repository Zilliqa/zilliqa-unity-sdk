using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-txbodies-for-txblock-ex
 */
public class GetTxnBodiesForTxBlockEx : MonoBehaviour
{
    const string METHOD = "GetTxnBodiesForTxBlockEx";

    public string TXBlockNumber = "1002353";
    public string pageNumber = "2";
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    public bool showDebug = true;

    [Serializable]
    struct GetTxnBodiesForTxBlockExRequest
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
            GetTxnBodiesForTxBlockExRequest getTxnBodiesForTxBlockEx = new GetTxnBodiesForTxBlockExRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = METHOD,
                paramsList = new List<string>()
            };
            getTxnBodiesForTxBlockEx.paramsList.Add(TXBlockNumber);
            getTxnBodiesForTxBlockEx.paramsList.Add(pageNumber);

            string json = JsonUtility.ToJson(getTxnBodiesForTxBlockEx);
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
