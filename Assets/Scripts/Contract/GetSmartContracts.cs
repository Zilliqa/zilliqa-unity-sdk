using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-contract-get-smartcontracts
 */
public class GetSmartContracts : MonoBehaviour
{
    const string METHOD = "GetSmartContracts";

    public string userAccount = "1eefc4f453539e5ee732b49eb4792b268c2f3908";
    public string apiUrl = "https://api.zilliqa.com/";//"https://dev-api.zilliqa.com/"
    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct GetSmartContractsRequest
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
            GetSmartContractsRequest getSmartContracts = new GetSmartContractsRequest
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
