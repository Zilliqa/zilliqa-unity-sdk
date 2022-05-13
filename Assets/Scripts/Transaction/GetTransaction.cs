using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-tx
 */
public class GetTransaction : ZilliqaMonoBehaviour
{
    const string METHOD = "GetTransaction";

    public string transactionID = "fba9cbbaef13767679cc77e62811e74a1a53858db91606a3c7d9ac8ff6aa5a0c";

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
        ZilRequest getTxBlockListingReq = new ZilRequest(METHOD, new object[] { transactionID });
        yield return StartCoroutine(PostRequest<GetTransactionResponse>(getTxBlockListingReq, (response, error) =>
        {
            if (response.result != null)
            {
                string debugStr = "ID " + response.result.ID + "\n" +
                                "amount " + response.result.amount + "\n" +
                                "gasLimit " + response.result.gasLimit + "\n" +
                                "gasPrice " + response.result.gasPrice + "\n" +
                                "nonce " + response.result.nonce + "\n" +
                                "cumulative_gas " + response.result.receipt + "\n" +
                                "epoch_num " + response.result.receipt.epoch_num + "\n" +
                                "success " + response.result.receipt.success +
                                "senderPubKey " + response.result.pubKey + "\n" +
                                "signature " + response.result.signature + "\n" +
                                "toAddr " + response.result.toAddr + "\n" +
                                "version " + response.result.version;

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
