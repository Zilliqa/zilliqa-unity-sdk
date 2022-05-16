using System;
using System.Collections;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Org.BouncyCastle.Math;
using Zilliqa.Core;
using Zilliqa.Utils;
using Zilliqa.Core.Crypto;
using Zilliqa.Requests;

public class TransferFrom : ZilliqaMonoBehaviour
{
    private const string CreateTransactionMethod = "CreateTransaction";
    private const string GetBalanceMethod = "GetBalance";

    [Header("Transaction Payload")]
    [SerializeField] private int version = 21823489;
    [SerializeField] private long nonce = 0;
    [SerializeField] private string toAddress = "0x638a429b1f0dc1dd206c3030295255d7dbf45501";
    [SerializeField] private string amount = "1000000000000";
    [SerializeField] private string gasPrice = "2000000000";
    [SerializeField] private string gasLimit = "50";
    [SerializeField] private string code = "";
    [SerializeField] private string data = "";
    [SerializeField] private bool priority = false;

    [Header("Keys Pair")]
    private string Address = "8254b2C9aCdf181d5d6796d63320fBb20D4Edd12";
    private string privateKey = "3375F915F3F9AE35E6B301B7670F53AD1A5BE15D8221EC7FD5E503F21D3450C8";
    private string publicKey;
    private bool autoNonce = true;

    [Header("Debug")]
    [SerializeField] private bool runAtStart = true;
    [SerializeField] private bool showDebug = true;

    [SerializeField] private float getTransactionStatusIntervalSeconds = 5f;
    [Tooltip("Set a negative number for infinite attempts")]
    [SerializeField] private int getTransactionStatusMaxAttempts = 50;

    public ECKeyPair ecKeyPair;

    private void Awake()
    {
        string checksummed = AddressUtils.ToCheckSumAddress(toAddress);
        publicKey = CryptoUtil.GetPublicKeyFromPrivateKey(privateKey, true);
        ecKeyPair = new ECKeyPair(new BigInteger(publicKey, 16), new BigInteger(privateKey, 16));
    }

    private void Start()
    {
        if (runAtStart)
            StartCoroutine(Transact());
    }



    private IEnumerator Transact()
    {
        //data needs to be a stringified json, more info on the structure can be found here:
        //https://dev.zilliqa.com/docs/apis/api-transaction-create-tx
        data = JsonConvert.SerializeObject(new ContractTransactionParams()
        {
            _tag = "TransferFrom",
            args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "to",
                                    type = "ByStr20",
                                    value = "0x"+Address.ToUpper()
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_id",
                                    type = "Uint256",
                                    value = "6"
                                }
                            }

        });
        Transaction transactionParam = new Transaction()
        {
            version = this.version,
            nonce = this.nonce,
            // the contract address needs to be checksummed
            toAddr = AddressUtils.ToCheckSumAddress(this.toAddress),
            amount = this.amount,
            pubKey = publicKey,
            gasPrice = this.gasPrice,
            gasLimit = this.gasLimit,
            code = this.code,
            priority = this.priority,
            data = this.data
        };
        
        // GetBalance rpc is being called to get nonce counter if autoNonce is used
        if (autoNonce)
        {
            if (string.IsNullOrEmpty(Address))
                Debug.LogError("Error: Failed to auto increase nonce. Please input wallet address.");
            else
            {
                ZilRequest getBalanceReq = new ZilRequest(GetBalanceMethod, Address);
                yield return StartCoroutine(PostRequest<GetBalanceResponse>(getBalanceReq, (response, error) =>
                    {
                        if (response.result != null)
                        {
                            nonce = response.result.nonce;
                            transactionParam.nonce = response.result.nonce + 1;
                            Debug.Log("Balance of " + Address + " : " + response.result.balance);
                        }
                        else if (error != null)
                        {
                            Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
                        }
                    }
                ));
            }
        }
        //Encode the protobuf object to byte array
        byte[] message = transactionParam.Encode();
        // sign the transaction based on the payload
        Signature signature = Schnorr.Sign(ecKeyPair, message);
        transactionParam.signature = signature.ToString().ToLower();

        ZilRequest createTxReq = new ZilRequest(CreateTransactionMethod, new object[] { transactionParam });
        StartCoroutine(PostRequest<CreateTransactionResponse>(createTxReq, (response, error) =>
            {
                var result = response.result;
                if (response.result != null)
                {
                    if (showDebug)
                        Debug.Log("Info: " + result.Info + "\n" + "Tx hash: " + "0x" + result.TranID);
                    StartCoroutine(ListenForTransactionStatusUpdate(result.TranID));
                }
                else if (error != null)
                {
                    Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
                }
            }
        ));
    }

    private IEnumerator ListenForTransactionStatusUpdate(string transactionID)
    {
        int currentAttemptCount = 0;
        bool hasFinalState = false;
        while ((currentAttemptCount < getTransactionStatusMaxAttempts || getTransactionStatusMaxAttempts < 0) && !hasFinalState)
        {
            yield return new WaitForSeconds(getTransactionStatusIntervalSeconds);
            yield return GetTransactionStatus(transactionID, (txStatus) =>
            {
                hasFinalState = txStatus.HasFinalState;
            });
            currentAttemptCount++;
        }
    }

    private IEnumerator GetTransactionStatus(string transactionID, Action<GetTransactionStatusPayload> txStatus = null)
    {
        ZilRequest getTxBlockListingReq = new ZilRequest("GetTransactionStatus", new object[] { transactionID });
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
                txStatus?.Invoke(response.result);
            }
            else if (error != null)
            {
                Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
            }
        }
            ));
    }
}
