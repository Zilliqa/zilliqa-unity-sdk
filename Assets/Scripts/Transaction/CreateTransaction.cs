using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math;

public class CreateTransaction : ZilliqaMonoBehaviour
{
    private const string CreateTransactionMethod = "CreateTransaction";
    private const string GetBalanceMethod = "GetBalance";

    [Header("Transaction Payload")]
    [SerializeField] private int version = 21823489;
    [SerializeField] private long nonce = 0;
    [SerializeField] private string toAddress = "0x3aca9ce184ee8237bd6426553aec5beca53c76207754a0b3f8ebebdb4ee553a6";
    [SerializeField] private string amount = "1000000000000";
    [SerializeField] private string gasPrice = "2000000000";
    [SerializeField] private string gasLimit = "50";
    [SerializeField] private string code = "";
    [SerializeField] private string data = "";
    [SerializeField] private bool priority = false;

    [Header("Keys Pair")]
    private string publicKey = "0x3a869435fd3B34d313B0C5BA5cd478c8bD0B90aC";
    private string privateKey = "0x0899282aaf67e341dc618cbfde25abdbe14f8fc17ec0fc142ebaa6544075ffaa";
    
    private bool autoNonce = true;



    [Header("Debug")]
    [SerializeField] private bool runAtStart = true;
    [SerializeField] private bool showDebug = true;

    private ECKeyPair ecKeyPair;

    private void Awake()
    {
        
        var pubk = new BigInteger(1, Encoding.UTF8.GetBytes(publicKey));
        var prik = new BigInteger(1, Encoding.UTF8.GetBytes(privateKey));
        ecKeyPair = new ECKeyPair(pubk, prik);
    }

    private void Start()
    {
        if (runAtStart)
            StartCoroutine(Transact());
    }



    private IEnumerator Transact()
    {

        //var txParam = new ContractTransactionParams[]
        //           {
        //                new ContractTransactionParams()
        //                {
        //                    _tag = "Mint",
        //                    args = new ContractTransitionArg[]
        //                    {
        //                        new ContractTransitionArg()
        //                        {
        //                            vname = "to",
        //                            type = "ByStr20",
        //                            value = publicKey
        //                        },
        //                        new ContractTransitionArg()
        //                        {
        //                            vname = "token_uri",
        //                            type = "String",
        //                            value = "1"
        //                        }
        //                    }
        //                }
        //           };

        //string json = JsonConvert.SerializeObject(txParam);

        //Debug.Log(json);
        Transaction transactionParam = new Transaction()
        {
            version = this.version,
            nonce = this.nonce,
            toAddr = this.toAddress,
            amount = this.amount,
            pubKey = this.publicKey,
            gasPrice = this.gasPrice,
            gasLimit = this.gasLimit,
            code = this.code,
            priority = this.priority,
            data = new ContractTransactionParams[]
                   {
                        new ContractTransactionParams()
                        {
                            _tag = "Mint",
                            args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "to",
                                    type = "ByStr20",
                                    value = publicKey
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_uri",
                                    type = "String",
                                    value = "1"
                                }
                            }
                        }
                   }
        };

        // GetBalance rpc is being called to get nonce counter if autoNonce is used
        if (autoNonce)
        {
            if (string.IsNullOrEmpty(publicKey))
                Debug.LogError("Error: Failed to auto increase nonce. Please input wallet address.");
            else
            {

                ZilRequest getBalanceReq = new ZilRequest(GetBalanceMethod, publicKey);
                yield return StartCoroutine(PostRequest<GetBalanceResponse>(getBalanceReq, (response, error) =>
                    {
                        if (response.result != null)
                        {
                            transactionParam.nonce = response.result.nonce + 1;
                        }
                        else if (error != null)
                        {
                            Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
                        }
                    }
                ));
            }
        }

        // sign the transaction based on the payload
        byte[] message = transactionParam.Encode();
        Signature signature = Schnorr.Sign(ecKeyPair, message);
        transactionParam.signature = signature.ToString().ToLower();

        if (showDebug)
            Debug.Log("Signature: " + transactionParam.signature);

        ZilRequest createTxReq = new ZilRequest(CreateTransactionMethod, new object[] { transactionParam });
        StartCoroutine(PostRequest<CreateTransactionResponse>(createTxReq, (response, error) =>
            {
                var result = response.result;
                if (response.result != null)
                {
                    if (showDebug)
                        Debug.Log("Info: " + result.Info + "\n" + "Tx hash: " + "0x" + result.TranID);
                }
                else if (error != null)
                {
                    Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
                }
            }
        ));
    }
}
