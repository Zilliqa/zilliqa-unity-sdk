using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-account-get-balance
 */
public class TxBlockListing : ZilliqaMonoBehaviour
{
    const string METHOD = "TxBlockListing";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetTxBlockListing
    {
        public int id;
        public string jsonrpc;
        public string method;
        public List<int> paramsList;
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
        GetTxBlockListing getTxBlockListing = new GetTxBlockListing
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<int>()
        };
        getTxBlockListing.paramsList.Add(1);

        string json = JsonUtility.ToJson(getTxBlockListing);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest getTxBlockListingReq = new ZilRequest(METHOD, new object[] { 1 });
        yield return StartCoroutine(PostRequest<GetTxBlockListingResponse>(getTxBlockListingReq, (response, error) =>
        {
            if (response.result != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Block List:" + Environment.NewLine);
                foreach (TransactionBlock item in response.result.data)
                {
                    sb.Append("BlockNum:" + item.BlockNum+Environment.NewLine);
                    sb.Append("Hash:    " + item.Hash + Environment.NewLine);
                }
                sb.Append("maxPages:" + response.result.maxPages);
                Debug.Log(sb.ToString());
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
