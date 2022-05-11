using Newtonsoft.Json;
using System;

[Serializable]
public class CreateTransactionSignedResponse : ZilResponse
{
    public CreateTransactionSignedPayload result;
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