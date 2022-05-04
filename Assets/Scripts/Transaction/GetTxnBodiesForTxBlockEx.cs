using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-txbodies-for-txblock-ex
 */
public class GetTxnBodiesForTxBlockEx : ZilliqaMonoBehaviour
{
    const string METHOD = "GetTxnBodiesForTxBlockEx";

    public string TXBlockNumber = "1002353";
    public string pageNumber = "2";
    
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
        ZilRequest getTxBlockListingReq = new ZilRequest(METHOD, new object[] { TXBlockNumber, pageNumber });
        yield return StartCoroutine(PostRequest<GetTxnBodiesForTxBlockExResponse>(getTxBlockListingReq, (response, error) =>
        {
            if (response.result != null)
            {
                string debugStr = "Current page " + response.result.CurrPage + "\nPages count " + response.result.NumPages + "\n";
                debugStr += "Transactions";
                for (int i = 0; i < response.result.Transactions.Length; i++)
                {
                    debugStr += "\nID " + response.result.Transactions[i].ID + "\n" +
                                "amount " + response.result.Transactions[i].amount + "\n" +
                                "gasLimit " + response.result.Transactions[i].gasLimit + "\n" +
                                "gasPrice " + response.result.Transactions[i].gasPrice + "\n" +
                                "nonce " + response.result.Transactions[i].nonce + "\n" +
                                "cumulative_gas " + response.result.Transactions[i].receipt + "\n" +
                                "epoch_num " + response.result.Transactions[i].receipt.epoch_num + "\n" +
                                "success " + response.result.Transactions[i].receipt.success +
                                "senderPubKey " + response.result.Transactions[i].pubKey + "\n" +
                                "signature " + response.result.Transactions[i].signature + "\n" +
                                "toAddr " + response.result.Transactions[i].toAddr + "\n" +
                                "version " + response.result.Transactions[i].version + "\n";
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
