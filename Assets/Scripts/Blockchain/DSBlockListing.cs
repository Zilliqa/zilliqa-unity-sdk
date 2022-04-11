using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-ds-block-listing
 */
public class DSBlockListing : MonoBehaviour
{
    const string METHOD = "DSBlockListing";

    public int pageNumber = 1;
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    public bool showDebug = true;

    [Serializable]
    struct DSBlockListingRequest
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
            DSBlockListingRequest DSBlockListing = new DSBlockListingRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = METHOD,
                paramsList = new List<int>()
            };
            DSBlockListing.paramsList.Add(pageNumber);

            string json = JsonUtility.ToJson(DSBlockListing);
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
