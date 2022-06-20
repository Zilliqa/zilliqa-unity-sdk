using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-get-ds-block
 */
public class GetDsBlock : ZilliqaMonoBehaviour
{
    const string METHOD = "GetDsBlock";

    public string DSBlockNumber = "9000";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetDsBlockRequest
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
        ZilRequest req = new ZilRequest(METHOD, new object[] { DSBlockNumber });
        yield return StartCoroutine(PostRequest<GetDSBlockResponse>(req, (response, error) =>
        {
            if (response.result != null)
            {
                string debugStr = "BlockNum " + response.result.header.BlockNum + "\n" +
                                  "Difficulty " + response.result.header.Difficulty + "\n" +
                                  "DifficultyDS " + response.result.header.DifficultyDS + "\n" +
                                  "GasPrice " + response.result.header.GasPrice + "\n" +
                                  "LeaderPubKey " + response.result.header.LeaderPubKey + "\n"+
                                  "PoWWinners ";
                foreach (string str in response.result.header.PoWWinners) {
                      debugStr += "     " + str + "\n";
                }

                debugStr+=        "PrevHash " + response.result.header.PrevHash + "\n" +
                                  "Timestamp " + response.result.header.Timestamp + "\n" +
                                  "Signature " + response.result.signature;

                Debug.Log(METHOD + " result " + debugStr);
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
