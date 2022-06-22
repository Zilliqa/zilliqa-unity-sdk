using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zilliqa.Requests
{
    public class GetTransactionStatusPayload : ResponsePayload
    {
        [JsonProperty("ID")]
        public string Id;

        [JsonProperty("_id")]
        [JsonConverter(typeof(ObjectIdConverter))]
        public string Oid;

        [JsonProperty("amount")]
        public string Amount;

        [JsonProperty("data")]
        public string Data;

        [JsonProperty("epochInserted")]
        public string EpochInserted;

        [JsonProperty("epochUpdated")]
        public string EpochUpdated;

        [JsonProperty("gasLimit")]
        public string GasLimit;

        [JsonProperty("gasPrice")]
        public string GasPrice;

        [JsonProperty("lastModified")]
        public string LastModified;

        [JsonProperty("modificationState")]
        public TransactionModificationState ModificationState;

        [JsonProperty("nonce")]
        public string Nonce;

        [JsonProperty("senderAddr")]
        public string SenderAddr;

        [JsonProperty("signature")]
        public string Signature;

        [JsonProperty("status")]
        public TransactionStatus Status;

        [JsonProperty("success")]
        public bool Success;

        [JsonProperty("toAddr")]
        public string ToAddr;

        [JsonProperty("version")]
        public string Version;

        /// <summary>
        /// Final state achieved status:
        /// true : achieved
        /// false : not yet, it is pending
        /// </summary>
        public bool HasFinalState => ModificationState == TransactionModificationState.Final;
    }

    public enum TransactionModificationState
    {
        Dispatched = 0,
        Pending = 1,
        Final = 2,
    }

    public enum TransactionStatus
    {
        Pending_Dispatched = 1,
        Pending_SoftConfirmed = 2,
        Pending_HighNounce = 4,
        Pending_GasLimitExceeded = 5,
        Pending_ConsensusFailure = 6,
        Confirmed = 3,
        Rejected_MathError = 10,
        Rejected_ScillaInvokeError = 11,
        Rejected_ContractAccountInitError = 12,
        Rejected_InvalidSourceAccount = 13,
        Rejected_GasLimitHigherThanShardGasLimit = 14,
        Rejected_UnknownTransactionType = 15,
        Rejected_TransactionSentToWrongShard = 16,
        Rejected_ContractAndSOurceAccountCrossShardIssue = 17,
        Rejected_CodeSizeExceededLimit = 18,
        Rejected_TransactionVerificationFailed = 19,
        Rejected_GasLimitTooLow = 20,
        Rejected_InsufficientBalance = 21,
        Rejected_InsufficientGas = 22,
        Rejected_DuplicateTransaction = 23,
        Rejected_TransactionWithSameNounceButSameOrHigherGasExists = 24,
        Rejected_InvalidDestAddress = 25,
        Rejected_FailedToAddContractToState = 26,
        Rejected_NounceIsLowerThanExpected = 27,
        Rejected_InternalError = 255,

    }
}
