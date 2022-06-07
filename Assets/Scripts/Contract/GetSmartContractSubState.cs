using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Zilliqa.Requests;
using Zilliqa.Utils;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-smartcontract-substate
 */
public class GetSmartContractSubState : ZilliqaMonoBehaviour
{
    const string METHOD = "GetSmartContractSubState";

    public string contractAddress = "173Ca6770Aa56EB00511Dac8e6E13B3D7f16a5a5";
    public string variableName = "sell_orders";
    public string indices = "{ \"9bfec715a6bd658fcb62b0f8cc9bfa2ade71434a\" }";
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
        ZilRequest GetSmartContractSubStateReq = new ZilRequest(METHOD, new object[] { AddressUtils.RemovePrefix_0x(TestWallets.FixedPriceSmartContract0), variableName,new object[] { } });
        yield return StartCoroutine(PostRequest<GetSmartContractSubStateResponse>(GetSmartContractSubStateReq, (response, error) =>
        {
            if (response.result != null)
            {
                Debug.Log("result " + response.result[variableName]);
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