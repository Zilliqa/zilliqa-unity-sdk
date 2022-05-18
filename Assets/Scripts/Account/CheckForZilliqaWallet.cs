using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zilliqa.Requests;

public class CheckForZilliqaWallet : ZilliqaMonoBehaviour
{
    const string METHOD = "GetBalance";

    public string accountID = "3a869435fd3B34d313B0C5BA5cd478c8bD0B90aC";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetBalanceRequest
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
        
        ZilRequest getBalanceReq = new ZilRequest(METHOD, new object[] { accountID });
        yield return StartCoroutine(PostRequest<GetBalanceResponse>(getBalanceReq, (response, error) =>
        {
            if (response.result != null)
            {
                Debug.Log("This is a Zilliqa wallet");
            }
            else if (error != null)
            {
                Debug.Log("This is not a Zilliqa wallet:\n Error code: " + error.code + "\n" + "Message: " + error.message);
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
