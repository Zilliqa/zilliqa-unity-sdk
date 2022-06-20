using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-smartcontract-code
 */
public class GetSmartContractCode : ZilliqaMonoBehaviour
{
    const string METHOD = "GetSmartContractCode";
    public string transactionID = "173Ca6770Aa56EB00511Dac8e6E13B3D7f16a5a5";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds


    [Serializable]
    struct GetSmartContractCodeStruct
    {
        public int id;
        public string jsonrpc;
        public string method;
        public List<string> paramsList;
    }

    void Start()
    {
        if (runAtStart)
            StartCoroutine(RunMethod());

        if (runForSeveralTimes)
            StartCoroutine(RunMethodCoroutine());
    }

    IEnumerator RunMethod()
    {
        GetSmartContractCodeStruct getSmartContractCode = new GetSmartContractCodeStruct
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getSmartContractCode.paramsList.Add(transactionID);

        string json = JsonUtility.ToJson(getSmartContractCode);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest GetSmartContractCodeReq = new ZilRequest(METHOD, new object[] { transactionID });
        yield return StartCoroutine(PostRequest<GetSmartContractCodeResponse>(GetSmartContractCodeReq, (response, error) =>
        {
            if (response.result != null)
            {
                Debug.Log("smart contract::\n" + response.result.code);
            }
            else if (error != null)
            {
                Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
            }
        }
            ));
    }

    IEnumerator RunMethodCoroutine()
    {
        //if run at start also enabled, then wait before first batch run
        if (runAtStart)
            yield return new WaitForSeconds(runDelay);

        for (int i = 1; i <= runTimes; i++)
        {
            yield return RunMethod();
            yield return new WaitForSeconds(runDelay);
        }
    }
}