﻿using UnityEngine;
using System;
using System.Collections.Generic;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-smartcontract-substate
 */
public class GetSmartContractSubState : MonoBehaviour
{
    const string METHOD = "GetSmartContractSubState";

    public string contractAddress = "173Ca6770Aa56EB00511Dac8e6E13B3D7f16a5a5";
    public string variableName = "admins";
    public string indices = "{ \"9bfec715a6bd658fcb62b0f8cc9bfa2ade71434a\" }";
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    public bool showDebug = true;

    [Serializable]
    struct GetSmartContractSubStateRequest
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
            GetSmartContractSubStateRequest getSmartContractSubState = new GetSmartContractSubStateRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = METHOD,
                paramsList = new List<string>()
            };

            getSmartContractSubState.paramsList.Add(contractAddress);
            getSmartContractSubState.paramsList.Add(variableName);
            getSmartContractSubState.paramsList.Add(indices);

            string json = JsonUtility.ToJson(getSmartContractSubState);
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