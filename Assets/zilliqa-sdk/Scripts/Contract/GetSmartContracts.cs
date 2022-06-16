using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-smartcontracts
 */
public class GetSmartContracts : ZilliqaMonoBehaviour
{
    const string METHOD = "GetSmartContracts";

    public string userAccount = "173Ca6770Aa56EB00511Dac8e6E13B3D7f16a5a5";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds


    [Serializable]
    struct GetSmartContractsStruct
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
        GetSmartContractsStruct getSmartContracts = new GetSmartContractsStruct
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getSmartContracts.paramsList.Add(userAccount);

        string json = JsonUtility.ToJson(getSmartContracts);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest GetSmartContractsReq = new ZilRequest(METHOD, new object[] { userAccount });
        yield return StartCoroutine(PostRequest<GetSmartContractsResponse>(GetSmartContractsReq, (response, error) =>
        {
            if (response.result != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Get smart contract init:" + Environment.NewLine);
                foreach (GetSmartContractsPayload contract in response.result)
                {
                    sb.Append(contract.address + Environment.NewLine);
                }
                Debug.Log(sb.ToString());
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