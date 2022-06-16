using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-get-miner-info
 */
public class GetMinerInfo : ZilliqaMonoBehaviour
{
    const string METHOD = "GetMinerInfo";

    public string DSBlockNumber = "5500";
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
        ZilRequest req = new ZilRequest(METHOD, new object[] { DSBlockNumber });
        yield return StartCoroutine(PostRequest<GetMinerInfoResponse>(req, (response, error) =>
        {
            if (response.result != null)
            {
                var debugStr = "\n dscommittee: \n";
                for (int i = 0; i < response.result.dscommittee.Length; i++)
                {
                    debugStr += response.result.dscommittee[i] + "\n";
                }
                debugStr += "shards: \n";
                for (int i = 0; i < response.result.shards.Length; i++)
                {
                    debugStr += "\n nodes:\n";
                    for (int j = 0; j < response.result.shards[i].nodes.Length; j++)
                    {
                        debugStr += "  "+response.result.shards[i].nodes[j] + "\n";
                    }
                    debugStr += " shards size "+response.result.shards[i].size + " ";
                }
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
