using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-get-blockchain-info
 */
public class GetBlockchainInfo : ZilliqaMonoBehaviour
{
    const string METHOD = "GetBlockchainInfo";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetBlockchainInfoRequest
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
        ZilRequest getBlockchainInfoReq = new ZilRequest(METHOD, new object[] { });
        yield return StartCoroutine(PostRequest<GetBlockchainInfoResponse>(getBlockchainInfoReq, (response, error) =>
        {
            if (response.result != null)
            {
                string debugStr = "CurrentDSEpoch" + response.result.CurrentDSEpoch + "\n" + 
                                    "CurrentMiniEpoch" + response.result.CurrentMiniEpoch + "\n" +
                                    "DSBlockRate" + response.result.DSBlockRate + "\n" +
                                    "NumDSBlocks" + response.result.NumDSBlocks + "\n" +
                                    "NumPeers" + response.result.NumPeers + "\n" +
                                    "NumTransactions" + response.result.NumTransactions + "\n" +
                                    "NumTxBlocks" + response.result.NumTxBlocks + "\n" +
                                    "NumTxnsDSEpoch" + response.result.NumTxnsDSEpoch + "\n" +
                                    "NumTxnsTxEpoch" + response.result.NumTxnsTxEpoch + "\n" +
                                    "ShardingStructure" + response.result.ShardingStructure.ToString() + "\n" +
                                    "TransactionRate" + response.result.TransactionRate + "\n" +
                                    "TxBlockRate" + response.result.TxBlockRate;
                Debug.Log(debugStr);

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
            StartCoroutine(RunMethod());
            yield return new WaitForSeconds(runDelay);
        }
    }
}
