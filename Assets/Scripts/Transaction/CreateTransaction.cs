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
    [SerializeField] private string toAddress = "b85B7B67288Ed648AB817F477B50FB45CA8E432b";
    [SerializeField] private string amount = "1000000000000";
    [SerializeField] private string gasPrice = "2000000000";
    [SerializeField] private string gasLimit = "50";
    [SerializeField] private string code = "";
    [SerializeField] private string data = "";
    [SerializeField] private bool priority = false;

    [Header("Keys Pair")]
    [SerializeField] private string publicKey = "03cbf8f0cb1ca9a27a1a7477bcd17577237d2a44e6a5b89224f45554cf6cd3734c";
    [SerializeField] private string privateKey = "76ba8a7f35bca7714feb842f746c0467fc66150df4d686fe83fcdc497f972e55";
    [SerializeField] private string walletAddress = "8Abea7C16e71750D493Eec2F1093A2f38d191628";
    [SerializeField] private bool autoNonce = true;



    [Header("Debug")]
    [SerializeField] private bool runAtStart = true;
    [SerializeField] private bool showDebug = true;

    private ECKeyPair ecKeyPair;

    private void Awake()
    {
        ecKeyPair = new ECKeyPair(new BigInteger(publicKey, 16), new BigInteger(privateKey, 16));
    }

    private void Start()
    {
        if (runAtStart)
            StartCoroutine(Transact());
    }



    private IEnumerator Transact()
    {

        var txParam = new ContractTransactionData[]
                   {
                        new ContractTransactionData()
                        {
                            _tag = "Mint",
                            args = new ContractTransitionArg[]
                            {
                                new ContractTransitionArg()
                                {
                                    vname = "to",
                                    type = "ByStr20",
                                    value = "0xb643b429d53a9ed8be8a0b8883edd9b6835d5fc1"
                                },
                                new ContractTransitionArg()
                                {
                                    vname = "token_uri",
                                    type = "String",
                                    value = "https://ivefwfclqyyavklisqgz.supabase.co/storage/v1/object/public/nftstorage/collection_example/metadata/4"
                                }
                            }
                        }
                   };

        string json = JsonConvert.SerializeObject(txParam);

        Debug.Log(json);
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
            data = new ContractTransactionData[]
                {
                    new ContractTransactionData()
                    {
                        _tag = "Mint",
                        args = new ContractTransitionArg[]
                        {
                            new ContractTransitionArg()
                            {
                                vname = "to",
                                type = "ByStr20",
                                value = "0xb643b429d53a9ed8be8a0b8883edd9b6835d5fc1"
                            },
                            new ContractTransitionArg()
                            {
                                vname = "token_uri",
                                type = "String",
                                value = "https://ivefwfclqyyavklisqgz.supabase.co/storage/v1/object/public/nftstorage/collection_example/metadata/4"
                            }
                        }
                    }
                }
        };

        // GetBalance rpc is being called to get nonce counter if autoNonce is used
        if (autoNonce)
        {
            if (string.IsNullOrEmpty(walletAddress))
                Debug.LogError("Error: Failed to auto increase nonce. Please input wallet address.");
            else
            {
                
                ZilRequest getBalanceReq = new ZilRequest(GetBalanceMethod, walletAddress);
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
