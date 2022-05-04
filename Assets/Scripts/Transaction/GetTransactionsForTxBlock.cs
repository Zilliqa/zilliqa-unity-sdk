using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-txs-for-txblock
 */
public class GetTransactionsForTxBlock : ZilliqaMonoBehaviour
{
    const string METHOD = "GetTransactionsForTxBlock";

    public string TXBlockNumber = "1376404";

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
        ZilRequest getTxBlockListingReq = new ZilRequest(METHOD, new object[] { TXBlockNumber });
        yield return StartCoroutine(PostRequest<GetTransactionForTxBlockResponse>(getTxBlockListingReq, (response, error) =>
        {
            if (response.result != null)
            {
                string debugStr = "";

                for (int i = 0; i < response.result.Length; i++)
                {
                    for (int j = 0; j < response.result[i].Length; j++)
                    {
                        debugStr += response.result[i][j] + "\n"; 
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
