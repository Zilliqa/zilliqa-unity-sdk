using Newtonsoft.Json;
using System;
using UnityEngine;
namespace Zilliqa.Requests
{
    [Serializable]
    public class ResponsePayload { }

    [Serializable]
    public class CreateTransactionPayload : ResponsePayload
    {
        [SerializeField]
        public string Info;
        [SerializeField]
        public string TranID;
    }

    public class requestParam { }

    public class CreateTransactionRequestParams
    {
        public int version;
        public int nonce;
        public string toAddr;
        public string amount;
        public string pubKey;
        public string gasPrice;
        public string gasLimit;
        public string code;
        public ContractTransactionParams[] data;
        public string signature;
        public bool priority;
    }

    [Serializable]
    public class ContractTransactionParams
    {
        public string _tag;
        [JsonProperty("params")]
        public ContractTransitionArg[] args;
    }

    [Serializable]
    public class ContractTransitionArg
    {
        public string vname;
        public string type;
        public string value;
    }

    public class OrderContractTransitionArg : ContractTransitionArg
    {
        public new OrderParam value;
    }

    public class OrderParam
    {
        public ContractTransitionArg token_address;
        public ContractTransitionArg token_id;
        public ContractTransitionArg payment_token_address;
        public ContractTransitionArg start_amount;
        public ContractTransitionArg expiration_bnum;

    }
}