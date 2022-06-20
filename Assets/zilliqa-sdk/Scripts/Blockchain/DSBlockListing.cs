using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-ds-block-listing
 */
public class DSBlockListing : ZilliqaMonoBehaviour
{
    const string METHOD = "DSBlockListing";

    public int pageNumber = 1;
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
        ZilRequest dsBlockListingReq = new ZilRequest(METHOD, new object[] { pageNumber });
        yield return StartCoroutine(PostRequest<GetDSBlockListingResponse>(dsBlockListingReq, (response, error) =>
        {
            if (response.result != null)
            {
                string debugStr = METHOD + "\n";
                for (int i = 0; i < response.result.data.Length; i++)
                {
                    debugStr += "Block number: " + response.result.data[i].BlockNum + " Hash: " + response.result.data[i].Hash + "\n";
                }
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
            yield return RunMethod();
            yield return new WaitForSeconds(runDelay);
        }
    }
}
