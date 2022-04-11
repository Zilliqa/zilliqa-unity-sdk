using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-contractaddress-from-txid
 */
public class GetContractAddressFromTransactionID : MonoBehaviour
{
    const string METHOD = "GetContractAddressFromTransactionID";

    public string transactionID = "bc1f85fa950bf18bb8c5d92dca9398a47a5a5287b5fcdf110ae1278775a46991";
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    public bool showDebug = true;

    [Serializable]
    struct GetContractAddressFromTransactionIDRequest
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
            GetContractAddressFromTransactionIDRequest getContractAddressFromTransactionID = new GetContractAddressFromTransactionIDRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = METHOD,
                paramsList = new List<string>()
            };
            getContractAddressFromTransactionID.paramsList.Add(transactionID);

            string json = JsonUtility.ToJson(getContractAddressFromTransactionID);
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
