using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-smartcontract-init
 */
public class GetSmartContractInit : ZilliqaMonoBehaviour
{
    const string METHOD = "GetSmartContractInit";

    public string contractAddress = "173Ca6770Aa56EB00511Dac8e6E13B3D7f16a5a5";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds


    [Serializable]
    struct GetSmartContractInitStruct
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
        GetSmartContractInitStruct getSmartContractInitCode = new GetSmartContractInitStruct
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getSmartContractInitCode.paramsList.Add(contractAddress);

        string json = JsonUtility.ToJson(getSmartContractInitCode);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest GetSmartContractInitReq = new ZilRequest(METHOD, new object[] { contractAddress });
        yield return StartCoroutine(PostRequest<GetSmartContractInitResponse>(GetSmartContractInitReq, (response, error) =>
        {
            if (response.result != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Get smart contract init:" + Environment.NewLine);
                foreach(GetSmartContractInitPayload items in response.result)
                {
                    sb.Append("type:" + items.type+Environment.NewLine);
                    sb.Append("value:" + items.value + Environment.NewLine);
                    sb.Append("vname:" + items.vname + Environment.NewLine);
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