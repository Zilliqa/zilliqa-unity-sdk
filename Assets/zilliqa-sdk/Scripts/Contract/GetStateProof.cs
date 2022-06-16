using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-state-proof
 */
public class GetStateProof : ZilliqaMonoBehaviour
{
    const string METHOD = "GetStateProof";

    public string contractAddress = "173Ca6770Aa56EB00511Dac8e6E13B3D7f16a5a5";
    public string variableHash = "A0BD91DE66D97E6930118179BA4F1836C366C4CB3309A6B354D26F52ABB2AAC6";
    public string TxBlock = "1938200";

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
        ZilRequest getTxBlockListingReq = new ZilRequest(METHOD, new object[] { contractAddress, variableHash, TxBlock });
        yield return StartCoroutine(PostRequest<GetStateProofResponse>(getTxBlockListingReq, (response, error) =>
        {
            if (response.stateProof != null && response.accountProof != null)
            {
                string debugStr = "StateProof\n";
                if (response.stateProof != null)
                {
                    for (int i = 0; i < response.stateProof.Length; i++)
                    {
                        debugStr += response.stateProof[i] + "\n";
                    }
                }

                debugStr += "accountProof\n";
                if (response.accountProof != null)
                {
                    for (int i = 0; i < response.stateProof.Length; i++)
                    {
                        debugStr += response.accountProof[i] + "\n";
                    }
                }

                Debug.Log("Get state proof " + debugStr);
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
