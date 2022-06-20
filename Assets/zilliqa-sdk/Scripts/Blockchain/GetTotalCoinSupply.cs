using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-get-total-coin-supply
 */
public class GetTotalCoinSupply : ZilliqaMonoBehaviour

{
    const string METHOD = "GetTotalCoinSupply";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetTotalCoinSupplyStruct
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
        GetTotalCoinSupplyStruct getTotalCoinSupply = new GetTotalCoinSupplyStruct
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getTotalCoinSupply.paramsList.Add("");

        string json = JsonUtility.ToJson(getTotalCoinSupply);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest getTotalCoinSupplyReq = new ZilRequest(METHOD, new object[] { "" });
        yield return StartCoroutine(PostRequest<GetTotalCoinSupplyResponse>(getTotalCoinSupplyReq, (response, error) =>
        {
            if (response != null)
            {

                Debug.Log("Total coins supply: " + response.result.ToString());
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
