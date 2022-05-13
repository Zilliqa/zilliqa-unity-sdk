using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Zilliqa.Requests;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-get-transaction-status
 */
public class GetTransactionStatus : ZilliqaMonoBehaviour
{
    const string METHOD = "GetTransactionStatus";

    public string transactionID = "1bb178b023f816e950d862f6505cd79a32bb97e71fd78441cbc3486940a2e1b7";
    
    public bool showDebug = true;

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    void Start()
    {
        if (runAtStart)
            StartCoroutine(RunMethod());

        if (runForSeveralTimes)
            StartCoroutine(RunMethodCoroutine());
    }

    IEnumerator RunMethod()
    {
        ZilRequest getTxBlockListingReq = new ZilRequest(METHOD, new object[] { transactionID });
        yield return StartCoroutine(PostRequest<GetTransactionStatusResponse>(getTxBlockListingReq, (response, error) =>
        {
            if (response.result != null)
            {
                string debugStr = "{\n\t<b>Id</b>: " + response.result.Id + "\n" +
                    "\t<b>Oid</b>: " + response.result.Oid.ToString() + "\n" +
                    "\t<b>Amount</b>: " + response.result.Amount + "\n" +
                    "\t<b>Data</b>: " + response.result.Data + "\n" +
                    "\t<b>EpochInserted</b>: " + response.result.EpochInserted + "\n" +
                    "\t<b>EpochUpdated</b>: " + response.result.EpochUpdated + "\n" +
                    "\t<b>GasLimit</b>: " + response.result.GasLimit + "\n" +
                    "\t<b>GasPrice</b>: " + response.result.GasPrice + "\n" +
                    "\t<b>LastModified</b>: " + response.result.LastModified + "\n" +
                    "\t<b>ModificationState</b>: " + "<color=#39002e>" + response.result.ModificationState + "</color>\n" +
                    "\t<b>Nonce</b>: " + response.result.Nonce + "\n" +
                    "\t<b>SenderAddr</b>: " + response.result.SenderAddr + "\n" +
                    "\t<b>Signature</b>: " + response.result.Signature + "\n" +
                    "\t<b>Status</b>: " + "<color=#39002e>" + response.result.Status + "</color>\n" +
                    "\t<b>Success</b>: " + "<color=#39000d>" + response.result.Success + "</color>\n" +
                    "\t<b>ToAddr</b>: " + response.result.ToAddr + "\n" +
                    "\t<b>Version</b>: " + response.result.Version + "\n}";

                Debug.Log("Get transaction: " + debugStr);
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
