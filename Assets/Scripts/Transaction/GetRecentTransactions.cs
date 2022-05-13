using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-recent-txs
 */
public class GetRecentTransactions : ZilliqaMonoBehaviour
{
    const string METHOD = "GetRecentTransactions";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    void Start()
    {
        if (runAtStart)
            StartCoroutine(RunMethod());

        if (runForSeveralTimes)
            StartCoroutine(RunMethodCoroutine());
    }

    IEnumerator RunMethod()
    {
        ZilRequest getTxBlockListingReq = new ZilRequest(METHOD, new object[] { });
        yield return StartCoroutine(PostRequest<GetRecentTransactionsResponse>(getTxBlockListingReq, (response, error) =>
        {
            if (response.result != null)
            {
                var debugStr = "Hashes \n";
                for (int i = 0; i < response.result.TxnHashes.Length; i++)
                {
                    debugStr += response.result.TxnHashes[i] + "\n";
                }

                debugStr += "number " + response.result.number;
                Debug.Log("Recent transactions:" + debugStr);
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
