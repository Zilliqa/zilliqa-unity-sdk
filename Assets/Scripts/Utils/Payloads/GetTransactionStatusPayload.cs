using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int ModificationState;

    [JsonProperty("nonce")]
    public string Nonce;

    [JsonProperty("senderAddr")]
    public string SenderAddr;

    [JsonProperty("signature")]
    public string Signature;

    [JsonProperty("status")]
    public int Status;

    [JsonProperty("success")]
    public bool Success;

    [JsonProperty("toAddr")]
    public string ToAddr;

    [JsonProperty("version")]
    public string Version;
}
