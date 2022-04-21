using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math;

public class CreateTransactionSigned : MonoBehaviour
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

    [Header("Test Network")]
    [SerializeField] private string apiUrl = "https://dev-api.zilliqa.com/";//"https://api.zilliqa.com/"

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

    private IEnumerator PostRequest(ZilRequest request, Action<object, ZilResponse.Error> onComplete = null)
    {
        string json = request.ToJson();
        using UnityWebRequest webRequest = new UnityWebRequest(apiUrl, "POST");
        byte[] rawData = Encoding.UTF8.GetBytes(json);

        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.uploadHandler = new UploadHandlerRaw(rawData);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                var response = JsonConvert.DeserializeObject<ZilResponse>(webRequest.downloadHandler.text);
                onComplete?.Invoke(response.result, response.error);
                break;
            default:
                break;
        }
    }

    private IEnumerator Transact()
    {
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
            data = this.data,
            priority = this.priority,
        };

        // GetBalance rpc is being called to get nonce counter if autoNonce is used
        if (autoNonce)
        {
            if (string.IsNullOrEmpty(walletAddress))
                Debug.LogError("Error: Failed to auto increase nonce. Please input wallet address.");
            else
            {
                ZilRequest getBalanceReq = new ZilRequest(GetBalanceMethod, walletAddress);
                yield return StartCoroutine(PostRequest(getBalanceReq, (result, error) =>
                    {
                        if (result != null)
                        {
                            Balance balance = ((JObject)result).ToObject<Balance>();
                            transactionParam.nonce = balance.nonce + 1;
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
        StartCoroutine(PostRequest(createTxReq, (result, error) =>
            {
                if (result != null)
                {
                    Transaction.Response txResponse = ((JObject)result).ToObject<Transaction.Response>();
                    if (showDebug)
                        Debug.Log("Info: " + txResponse.Info + "\n" + "Tx hash: " + "0x" + txResponse.TranID);
                }
                else if (error != null)
                {
                    Debug.Log("Error code: " + error.code + "\n" + "Message: " + error.message);
                }
            }
        ));
    }
}
