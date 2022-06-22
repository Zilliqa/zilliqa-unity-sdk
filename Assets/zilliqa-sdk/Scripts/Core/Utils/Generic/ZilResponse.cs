using System;

/// <summary>
/// Base class for any type of response returned by a Zilliqa RPC.
/// </summary>
[Serializable]
public class ZilResponse
{
    public class Error
    {
        public int code;
        public object data;
        public string message;
    }

    public string id;
    public string jsonrpc;
    

    public Error error;
}
