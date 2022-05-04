using System;

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
