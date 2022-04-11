﻿using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-txs-for-txblock
 */
public class GetTransactionsForTxBlock : MonoBehaviour
{
    const string METHOD = "GetTransactionsForTxBlock";

    public string TXBlockNumber = "1376404";
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    public bool showDebug = true;

    [Serializable]
    struct GetTransactionsForTxBlockRequest
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
            GetTransactionsForTxBlockRequest getTransactionsForTxBlock = new GetTransactionsForTxBlockRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = METHOD,
                paramsList = new List<string>()
            };
            getTransactionsForTxBlock.paramsList.Add(TXBlockNumber);

            string json = JsonUtility.ToJson(getTransactionsForTxBlock);
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
