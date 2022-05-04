using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-account-get-balance
 */
public class GetBalance : ZilliqaMonoBehaviour
{
    const string METHOD = "GetBalance";

    public string accountID = "zil1fxrj57r92djsszx42xv76cqz3uc2t60tnepxvp";

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
        GetBalanceRequest getBalance = new GetBalanceRequest
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getBalance.paramsList.Add(accountID);

        string json = JsonUtility.ToJson(getBalance);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest getBalanceReq = new ZilRequest(METHOD, new object[] { accountID });
        yield return StartCoroutine(PostRequest<GetBalanceResponse>(getBalanceReq, (response, error) =>
        {
            if (response != null)
            {
                Debug.Log("Balance of " + accountID + " : " + response.result.balance);
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
