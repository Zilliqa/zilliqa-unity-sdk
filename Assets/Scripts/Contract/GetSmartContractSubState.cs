using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-smartcontract-substate
 */
public class GetSmartContractSubState : ZilliqaMonoBehaviour
{
    const string METHOD = "GetSmartContractSubState";

    public string contractAddress = "173Ca6770Aa56EB00511Dac8e6E13B3D7f16a5a5";
    public string variableName = "admins";
    public string indices = "{ \"9bfec715a6bd658fcb62b0f8cc9bfa2ade71434a\" }";
    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds


    [Serializable]
    struct GetSmartContractSubStateStruct
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
        GetSmartContractSubStateStruct getSmartContractSubState = new GetSmartContractSubStateStruct
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getSmartContractSubState.paramsList.Add(contractAddress);
        getSmartContractSubState.paramsList.Add(variableName);
        getSmartContractSubState.paramsList.Add(indices);

        string json = JsonUtility.ToJson(getSmartContractSubState);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest GetSmartContractSubStateReq = new ZilRequest(METHOD, new object[] { contractAddress, variableName,new object[] { } });
        yield return StartCoroutine(PostRequest<GetSmartContractSubStateResponse>(GetSmartContractSubStateReq, (response, error) =>
        {
            if (response.result != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Get smart contract substate ???:" + Environment.NewLine);
                /*sb.Append("_balance:" + response.result._balance + Environment.NewLine);
                sb.Append("admin:" + response.result.admin + Environment.NewLine);
                sb.Append("allowances:" + Environment.NewLine);
                int counter = 0;
                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in response.result.allowances)
                {
                    Dictionary<string, string> kvp2 = kvp.Value;
                    sb.Append(kvp.Key + ":{" + kvp2.First().Key + ":" + kvp2.First().Value + "}" + Environment.NewLine);
                    counter++;
                    if (counter > 5)
                    {
                        break;
                    }
                }
                counter = 0;
                sb.Append("Balances:" + Environment.NewLine);
                foreach (KeyValuePair<string, string> kvp in response.result.balances)
                {
                    sb.Append(kvp.Key + ":" + kvp.Value + Environment.NewLine);
                    counter++;
                    if (counter > 5)
                    {
                        break;
                    }
                }*/
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