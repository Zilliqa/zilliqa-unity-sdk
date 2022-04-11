using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-tx-block-listing
 */
public class TxBlockListing : MonoBehaviour
{
    const string METHOD = "TxBlockListing";

    public int pageNumber = 1;
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    public bool showDebug = true;

    [Serializable]
    struct TxBlockListingRequest
    {
        public int id;
        public string jsonrpc;
        public string method;
        public List<int> paramsList;
    }

    void Start()
    {
        try
        {
            TxBlockListingRequest TxBlockListing = new TxBlockListingRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = METHOD,
                paramsList = new List<int>()
            };
            TxBlockListing.paramsList.Add(pageNumber);

            string json = JsonUtility.ToJson(TxBlockListing);
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
