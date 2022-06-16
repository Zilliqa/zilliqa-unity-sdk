using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-contractaddress-from-txid
 */
public class GetContractAddressFromTransactionID : ZilliqaMonoBehaviour
{
    const string METHOD = "GetContractAddressFromTransactionID";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    public string transactionID = "bc1f85fa950bf18bb8c5d92dca9398a47a5a5287b5fcdf110ae1278775a46991";

    [Serializable]
    struct GetContractAddressFromTransactionIDStruct
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
        GetContractAddressFromTransactionIDStruct getContractAddressFromTransactionID = new GetContractAddressFromTransactionIDStruct
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getContractAddressFromTransactionID.paramsList.Add(transactionID);

        string json = JsonUtility.ToJson(getContractAddressFromTransactionID);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest getContractAddressFromTransactionIDReq = new ZilRequest(METHOD, new object[] { transactionID });
        yield return StartCoroutine(PostRequest<GetContractAddressFromTransactionIDResponse>(getContractAddressFromTransactionIDReq, (response, error) =>
        {
            if (response.result != null)
            {
                Debug.Log("Transaction ID:" + response.result);
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