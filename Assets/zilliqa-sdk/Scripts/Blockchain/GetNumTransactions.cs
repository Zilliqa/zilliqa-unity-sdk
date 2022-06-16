using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-get-num-tx
 */
public class GetNumTransactions : ZilliqaMonoBehaviour

{
    const string METHOD = "GetNumTransactions";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetNumTransactionsStruct
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
        GetNumTransactionsStruct getNumTransactions = new GetNumTransactionsStruct
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getNumTransactions.paramsList.Add("");

        string json = JsonUtility.ToJson(getNumTransactions);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest getNumTransactionsReq = new ZilRequest(METHOD, new object[] { "" });
        // Response is identical to GetNumTxBlocksResponse
        yield return StartCoroutine(PostRequest<GetNumTxBlocksResponse>(getNumTransactionsReq, (response, error) =>
        {
            if (response != null)
            {

                Debug.Log("Num Transactions: " + response.result);
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