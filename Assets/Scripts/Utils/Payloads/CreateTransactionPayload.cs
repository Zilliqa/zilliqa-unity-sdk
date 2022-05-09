using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResponsePayload {}

[Serializable]
public class CreateTransactionPayload : ResponsePayload
{
    [SerializeField]
    public string Info;
    [SerializeField]
    public string TranID;
}

public class requestParam { }

[Serializable]
public class ContractTransactionData
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