using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-txbodies-for-txblock
 */
public class GetTxnBodiesForTxBlock : ZilliqaMonoBehaviour
{
    const string METHOD = "GetTxnBodiesForTxBlock";

    public string TXBlockNumber = "1376404";
    
    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetTxnBodiesForTxBlockRequest
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
        ZilRequest getTxBlockListingReq = new ZilRequest(METHOD, new object[] { TXBlockNumber });
        yield return StartCoroutine(PostRequest<GetTxnBodiesForTxBlockResponse>(getTxBlockListingReq, (response, error) =>
        {
            if (response.result != null)
            {
                string debugStr = "Transactions";

                for (int i = 0; i < response.result.Length; i++)
                {
                    debugStr += "\nID " + response.result[i].ID + "\n" +
                                "amount " + response.result[i].amount + "\n" +
                                "gasLimit " + response.result[i].gasLimit + "\n" +
                                "gasPrice " + response.result[i].gasPrice + "\n" +
                                "nonce " + response.result[i].nonce + "\n" +
                                "cumulative_gas " + response.result[i].receipt + "\n" +
                                "epoch_num " + response.result[i].receipt.epoch_num + "\n" +
                                "success " + response.result[i].receipt.success +
                                "senderPubKey " + response.result[i].pubKey + "\n" +
                                "signature " + response.result[i].signature + "\n" +
                                "toAddr " + response.result[i].toAddr + "\n" +
                                "version " + response.result[i].version + "\n";
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
