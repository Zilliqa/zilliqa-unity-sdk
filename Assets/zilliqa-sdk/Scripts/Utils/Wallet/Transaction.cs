using Google.Protobuf;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using Zilliqa.Core;
using Zilliqa.Requests;

namespace Zilliqa.Utils
{
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
        //public ContractTransactionParams[] data;
        public string data;
        public string signature;
        public bool priority;

        public byte[] Encode()
        {
            BigInteger amount = BigInteger.ValueOf(long.Parse(this.amount));
            BigInteger gasPrice = BigInteger.ValueOf(long.Parse(this.gasPrice));

            ProtoTransaction protoTx = new ProtoTransaction();

            protoTx.Version = (uint)version;
            protoTx.Nonce = (ulong)nonce;
            protoTx.Toaddr = ByteString.CopyFrom(ByteUtil.HexStringToByteArray(toAddr.ToLower().Replace("0x", "")));
            protoTx.Pubkey = new ByteArray() { Data = ByteString.CopyFrom(ByteUtil.HexStringToByteArray(pubKey)) };
            protoTx.Amount = new ByteArray() { Data = ByteString.CopyFrom(BigIntegers.AsUnsignedByteArray(16, amount)) };
            protoTx.Gasprice = new ByteArray() { Data = ByteString.CopyFrom(BigIntegers.AsUnsignedByteArray(16, gasPrice)) };
            protoTx.Gaslimit = ulong.Parse(gasLimit);
            protoTx.Code = ByteString.CopyFrom(ByteUtil.HexStringToByteArray(code));
            protoTx.Data = ByteString.CopyFromUtf8(data);


            if (!string.IsNullOrEmpty(code))
            {
                protoTx.Code = ByteString.CopyFrom(Encoding.Default.GetBytes(code));
            }

            //if (!string.IsNullOrEmpty(data))
            //{
            //    protoTx.Data = ByteString.CopyFrom(System.Text.Encoding.Default.GetBytes(data));
            //}

            byte[] protoBytes = protoTx.ToByteArray();



            return protoBytes;
            //return Encoding.UTF8.GetBytes(protoTx.ToString().Replace(" ", ""));
        }
        public static byte[] ObjectToByteArray(System.Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
    [Serializable]
    public class ProtoParent
    {
        public ProtoTransaction parent;
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

    public class ZilpayTransaction
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
        //public ContractTransactionParams[] data;
        public ContractTransactionParams data;
        public string signature;
        public bool priority;
    }
}