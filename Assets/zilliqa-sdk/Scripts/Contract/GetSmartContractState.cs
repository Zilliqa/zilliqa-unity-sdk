using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-smartcontract-state
 */
public class GetSmartContractState : ZilliqaMonoBehaviour
{
    const string METHOD = "GetSmartContractState";

    public string contractAddress = "173Ca6770Aa56EB00511Dac8e6E13B3D7f16a5a5";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds


    [Serializable]
    struct GetSmartContractStateStruct
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
        GetSmartContractStateStruct getSmartContractState = new GetSmartContractStateStruct
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getSmartContractState.paramsList.Add(contractAddress);

        string json = JsonUtility.ToJson(getSmartContractState);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest GetSmartContractsReq = new ZilRequest(METHOD, new object[] { contractAddress });
        yield return StartCoroutine(PostRequest<GetSmartContractStateResponse>(GetSmartContractsReq, (response, error) =>
        {
            if (response.result != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Get smart contract state (output is truncated):" + Environment.NewLine);
                sb.Append("_balance:" + response.result._balance + Environment.NewLine);
                sb.Append("admin:" + response.result.admin + Environment.NewLine);
                sb.Append("allowances:" + Environment.NewLine);
                int counter = 0;
                foreach(KeyValuePair<string,Dictionary<string,string>> kvp in response.result.allowances)
                {
                    Dictionary<string, string> kvp2 = kvp.Value;
                    sb.Append(kvp.Key + ":{" + kvp2.First().Key + ":" + kvp2.First().Value+"}"+Environment.NewLine);
                    counter++;
                    if (counter > 5)
                    {
                        break;
                    }
                }
                counter = 0;
                sb.Append("Balances:"+Environment.NewLine);
                foreach (KeyValuePair<string, string> kvp in response.result.balances)
                {
                    sb.Append(kvp.Key + ":" + kvp.Value + Environment.NewLine);
                    counter++;
                    if (counter > 5)
                    {
                        break;
                    }
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