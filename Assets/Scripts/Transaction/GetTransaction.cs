using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-tx
 */
public class GetTransaction : MonoBehaviour
{
    const string METHOD = "GetTransaction";

    public string transactionID = "fba9cbbaef13767679cc77e62811e74a1a53858db91606a3c7d9ac8ff6aa5a0c";
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetTransactionRequest
    {
        public int id;
        public string jsonrpc;
        public string method;
        public List<string> paramsList;
    }

    void Start()
    {
        if (runAtStart)
            RunMethod();

        if (runForSeveralTimes)
            StartCoroutine(RunMethodCoroutine());
    }

    void RunMethod()
    {
        try
        {
            GetTransactionRequest getTransaction = new GetTransactionRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = METHOD,
                paramsList = new List<string>()
            };
            getTransaction.paramsList.Add(transactionID);

            string json = JsonUtility.ToJson(getTransaction);
            json = json.Replace("paramsList", "params");

            if (showDebug)
                Debug.Log(METHOD + ":\n" + json);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");
            
            byte[] pData = System.Text.Encoding.ASCII.GetBytes(json.ToCharArray());

            WWW api = new WWW(apiUrl, pData, headers);

            StartCoroutine(Utils.WaitForWWW(api, showDebug, METHOD));
        }
        catch (UnityException ex)
        {
            Debug.Log(ex.Message);
        }
    }

    IEnumerator RunMethodCoroutine()
    {
        //if run at start also enabled, then wait before first batch run
        if (runAtStart)
            yield return new WaitForSeconds(runDelay);

        for (int i = 1; i <= runTimes; i++)
        {
            RunMethod();
            yield return new WaitForSeconds(runDelay);
        }
    }
}
