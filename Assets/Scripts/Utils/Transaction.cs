using Google.Protobuf;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System.Text;

[System.Serializable]
public class Transaction
{
    public class Response
    {
        public string Info;
        public string TranID;
    }

    public int version;
    public long nonce;
    public string toAddr;
    public string amount;
    public string pubKey;
    public string gasPrice;
    public string gasLimit;
    public string code;
    public string data;
    public string signature;
    public bool priority;

    public byte[] Encode()
    {
        BigInteger amount = BigInteger.ValueOf(long.Parse(this.amount));
        BigInteger gasPrice = BigInteger.ValueOf(long.Parse(this.gasPrice));

        ProtoTransaction protoTx = new ProtoTransaction
        {
            Version = (uint)version,
            Nonce = (ulong)nonce,
            Toaddr = ByteString.CopyFrom(ByteUtility.HexStringToByteArray(toAddr.ToLower())),
            Pubkey = new ByteArray() { Data = ByteString.CopyFrom(ByteUtility.HexStringToByteArray(pubKey)) },
            Amount = new ByteArray() { Data = ByteString.CopyFrom(BigIntegers.AsUnsignedByteArray(16, amount)) },
            Gasprice = new ByteArray() { Data = ByteString.CopyFrom(BigIntegers.AsUnsignedByteArray(16, gasPrice)) },
            Gaslimit = ulong.Parse(gasLimit)
        };

        if (!string.IsNullOrEmpty(code))
        {
            protoTx.Code = ByteString.CopyFrom(Encoding.Default.GetBytes(code));
        }

        if (!string.IsNullOrEmpty(data))
        {
            protoTx.Data = ByteString.CopyFrom(System.Text.Encoding.Default.GetBytes(data));
        }

        return protoTx.ToByteArray();
    }

}

public class ProcessedTransaction : Transaction
{
    public string ID;
    public TransactionReceipt receipt;
}

public class TransactionReceipt
{
    public string cumulative_gas;
    public string epoch_num;
    public bool success;
}