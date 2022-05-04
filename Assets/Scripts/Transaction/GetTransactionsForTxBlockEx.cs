using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-txs-for-txblock-ex
 */
public class GetTransactionsForTxBlockEx : ZilliqaMonoBehaviour
{
    const string METHOD = "GetTransactionsForTxBlockEx";

    public string TXBlockNumber = "1002353";
    public string pageNumber = "2";
    
    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetTransactionsForTxBlockExRequest
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
        ZilRequest getTxBlockListingReq = new ZilRequest(METHOD, new object[] { TXBlockNumber, pageNumber });
        yield return StartCoroutine(PostRequest<GetTransactionsForTxBlockExResponse>(getTxBlockListingReq, (response, error) =>
        {
            if (response.result != null)
            {
                string debugStr = "Current page " + response.result.CurrPage + "\nPages count " + response.result.NumPages + "\n";
                debugStr += "Transactions\n";
                for (int i = 0; i < response.result.Transactions.Length; i++)
                {
                    for (int j = 0; j < response.result.Transactions[i].Length; j++)
                    {
                        debugStr += response.result.Transactions[i][j] + "\n";
                    }
                    debugStr += "\n";
                }

                Debug.Log("Get transaction: " + debugStr);
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
