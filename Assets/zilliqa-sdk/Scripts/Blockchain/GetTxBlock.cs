using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-blockchain-get-tx-block
 */
public class GetTxBlock : ZilliqaMonoBehaviour
{
    const string METHOD = "GetTxBlock";

    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    public string transactionBlock = "1002353";

    [Serializable]
    struct GetTxBlockStruct
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
        GetTxBlockStruct getTxBlock = new GetTxBlockStruct
        {
            id = 1,
            jsonrpc = "2.0",
            method = METHOD,
            paramsList = new List<string>()
        };
        getTxBlock.paramsList.Add(transactionBlock);

        string json = JsonUtility.ToJson(getTxBlock);
        json = json.Replace("paramsList", "params");

        if (showDebug)
            Debug.Log(METHOD + ":\n" + json);

        ZilRequest getTxBlockReq = new ZilRequest(METHOD, new object[] { transactionBlock });
        yield return StartCoroutine(PostRequest<GetTxBlockResponse>(getTxBlockReq, (response, error) =>
        {
            if (response != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Body:"+Environment.NewLine);
                sb.Append(" BlockHash:" + response.result.body.BlockHash+Environment.NewLine);
                sb.Append(" HeaderSign:" + response.result.body.HeaderSign + Environment.NewLine);
                foreach (MicroBlock mb in response.result.body.MicroBlockInfos)
                {
                    sb.Append("     MicroBlockHash:" + mb.MicroBlockHash + Environment.NewLine);
                    sb.Append("     MicroBlockShardId:" + mb.MicroBlockShardId + Environment.NewLine);
                    sb.Append("     MicroBlockTxnRootHash:" + mb.MicroBlockTxnRootHash + Environment.NewLine);
                }
                sb.Append("header:" + Environment.NewLine);
                sb.Append(" BlockNum:" + response.result.header.BlockNum + Environment.NewLine);
                sb.Append(" DSBlockNum:" + response.result.header.DSBlockNum + Environment.NewLine);
                sb.Append(" GasLimit:" + response.result.header.GasLimit + Environment.NewLine);
                sb.Append(" GasUsed:" + response.result.header.GasUsed + Environment.NewLine);
                sb.Append(" MbInfoHash:" + response.result.header.MbInfoHash + Environment.NewLine);
                sb.Append(" MinerPubKey:" + response.result.header.MinerPubKey + Environment.NewLine);
                sb.Append(" NumMicroBlocks:" + response.result.header.NumMicroBlocks.ToString() + Environment.NewLine);
                sb.Append(" NumPages:" + response.result.header.NumPages.ToString() + Environment.NewLine);
                sb.Append(" NumTxns:" + response.result.header.NumTxns.ToString() + Environment.NewLine);
                sb.Append(" PrevBlockHash:" + response.result.header.PrevBlockHash + Environment.NewLine);
                sb.Append(" Rewards:" + response.result.header.Rewards + Environment.NewLine);
                sb.Append(" StateDeltaHash:" + response.result.header.StateDeltaHash + Environment.NewLine);
                sb.Append(" StateRootHash:" + response.result.header.StateRootHash + Environment.NewLine);
                sb.Append(" Timestamp:" + response.result.header.Timestamp + Environment.NewLine);
                sb.Append(" TxnFees:" + response.result.header.TxnFees + Environment.NewLine);
                sb.Append(" Version:" + response.result.header.Version.ToString() + Environment.NewLine);
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
