using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

//external scripts:
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using System.Collections;

/*
 * Documentation:
 * https://dev.zilliqa.com/docs/apis/api-transaction-create-tx
 */
public class CreateTransaction : MonoBehaviour
{
    const string METHOD = "CreateTransaction";

    /*
     *  How to calculate the version parameter:
     *
     *  Chain(s)	       |CHAIN_ID    |VERSION
     *  -------------------|------------|---------
     *  Zilliqa mainnet	   |1           |65537
     *  Developer testnet  |333         |21823489
     *  Isolated server	   |222         |145489921
     *
     *  //int CHAIN_ID = 333;
     *  //int MSG_VERSION = 1;
     *  //int version = (CHAIN_ID << 16) + MSG_VERSION;
     *  
     */
    public int version = 21823489;
    public int nonce = 1;
    public string toAddr = "4BAF5faDA8e5Db92C3d3242618c5B47133AE003C";
    public string amount = "10000000";
    public string pubKey = "0205273e54f262f8717a687250591dcfb5755b8ce4e3bd340c7abefd0de1276574";
    public string gasPrice = "2000000000";
    public string gasLimit = "50";
    public string code = "";
    public string data = "";
    public string apiUrl = "https://dev-api.zilliqa.com/";//"https://api.zilliqa.com/";
    public bool showDebug = true;
    public string privKey = "e19d05c5452598e24caad4a0d85a49146f7be089515c905ae6a19e8a578a6930";

    public bool runAtStart = true;
    public bool runForSeveralTimes = true;
    public int runTimes = 10;
    public float runDelay = 5f;//seconds

    [Serializable]
    struct CreateTransactionRequest
    {
        public int id;
        public string jsonrpc;
        public string method;
        public List<CreateTransactionRequestParams> paramsList;
    }

    [Serializable]
    struct CreateTransactionRequestParams
    {
        public int version;
        public int nonce;
        public string toAddr;
        public string amount;
        public string pubKey;
        public string gasPrice;
        public string gasLimit;
        public string code;
        public string data;
        public string signature;
        public bool priority;
    }

    private SecureRandom random = new SecureRandom();
    private static readonly Org.BouncyCastle.Asn1.X9.X9ECParameters curve = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp256k1");
    private static readonly Org.BouncyCastle.Crypto.Parameters.ECDomainParameters domain = new Org.BouncyCastle.Crypto.Parameters.ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

    public BigInteger GenerateRandom()
    {

        BigInteger d;
        int minWeight = curve.N.BitLength >> 2;

        for (; ; )
        {
            d = new BigInteger(curve.N.BitLength, random);

            if (d.CompareTo(BigInteger.Two) < 0 || d.CompareTo(curve.N) >= 0)
                continue;

            if (WNafUtilities.GetNafWeight(d) < minWeight)
                continue;

            break;
        }

        return d;
    }

    byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }
    string GetSign(byte[] msg)
    {
        byte[] prvB = StringToByteArray(privKey);
        byte[] pubB = StringToByteArray(pubKey);
        
        Org.BouncyCastle.Math.BigInteger sk = new Org.BouncyCastle.Math.BigInteger(+1, prvB);
        Org.BouncyCastle.Math.EC.ECPoint q = domain.G.Multiply(sk);

        byte[] pubKeyX = q.Normalize().AffineXCoord.GetEncoded();
        byte[] pubKeyY = q.Normalize().AffineYCoord.GetEncoded();
        BigInteger k = GenerateRandom();
        
        Debug.Log("random generated:" + BitConverter.ToString(k.ToByteArray()).Replace("-", ""));

        ECPoint Q = domain.G.Multiply(k);
        Org.BouncyCastle.Crypto.Digests.Sha256Digest digester = new Org.BouncyCastle.Crypto.Digests.Sha256Digest();
        byte[] h = new byte[digester.GetDigestSize()];

        digester.BlockUpdate(Q.GetEncoded(true), 0, Q.GetEncoded(true).Length);
        digester.BlockUpdate(pubB, 0, pubB.Length);
        digester.BlockUpdate(msg, 0, msg.Length);

        digester.DoFinal(h, 0);

        Org.BouncyCastle.Math.BigInteger r = new Org.BouncyCastle.Math.BigInteger(+1, h);

        BigInteger s = r.Multiply(sk);
        s = k.Subtract(s);
        s = s.Mod(domain.n);
        string rt = BitConverter.ToString(r.ToByteArray()).Replace("-", "");
        if (rt.Length > 32)
        {
            rt = rt.Remove(0, 2);
        }
        string st = BitConverter.ToString(s.ToByteArray()).Replace("-", "");

        return rt + st;


    }

    string FillZero(string st, int fill)
    {
        string str = st;
        for (int i = 0; i < fill - st.Length; i++)
            str = "0" + str;
        
        return str;
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
            CreateTransactionRequest createTransaction = new CreateTransactionRequest
            {
                id = 1,
                jsonrpc = "2.0",
                method = METHOD,
                paramsList = new List<CreateTransactionRequestParams>()
            };

            string lcodeHex = BitConverter.ToString(StringToByteArray(code)).Replace("-", "");
            string ldataHex = BitConverter.ToString(StringToByteArray(data)).Replace("-", "");
            string lversion = BitConverter.ToString(BitConverter.GetBytes(version)).Replace("-", "");
            string lnonce = BitConverter.ToString(BitConverter.GetBytes(nonce)).Replace("-", "");
            string lamount = BitConverter.ToString(StringToByteArray(amount)).Replace("-", "");
            string lgasPrice = BitConverter.ToString(StringToByteArray(gasPrice.ToString())).Replace("-", "");
            string lgasLimit = BitConverter.ToString(StringToByteArray(gasLimit.ToString())).Replace("-", "");
            string lcodeLength = FillZero(code.Length.ToString(), 8);
            string ldataLength = FillZero(data.Length.ToString(), 8);
            lversion = FillZero(lversion, 64);
            lnonce = FillZero(lnonce, 64);
            lamount = FillZero(lamount, 64);
            lgasPrice = FillZero(lgasPrice, 64);
            lgasLimit = FillZero(lgasLimit, 64);

            CreateTransactionRequestParams ptmp = new CreateTransactionRequestParams() {
                version = version,
                nonce = nonce,
                toAddr = toAddr,
                amount = amount,
                pubKey = pubKey,
                gasPrice = gasPrice,
                gasLimit = gasLimit,
                code = code,
                data = data,

                /* Please note:
                 *
                 * Read more about signature generating here:
                 * https://dev.zilliqa.com/docs/apis/api-transaction-create-tx
                 *
                 * signature example:
                 * 29ad673848dcd7f5168f205f7a9fcd1e8109408e6c4d7d03e4e869317b9067e636b216a32314dd37176c35d51f9d4c24e0e519ba80e66206457c83c9029a490d
                 * 
                 * Any issues like "Unable to verify transaction" or "Invalid Signature size"
                 * come from the parameters you use, and/or the network upon which the request
                 * is being run.
                 */
                signature = GetSign(StringToByteArray(
                    lversion +
                    lnonce +
                    toAddr +
                    lamount +
                    pubKey +
                    lgasPrice +
                    lgasLimit +
                    lcodeLength +
                    lcodeHex +
                    ldataLength +
                    ldataHex
                    )),

                priority = false
            };

            createTransaction.paramsList.Add(ptmp);
            
            string json = JsonUtility.ToJson(createTransaction);
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
