using Newtonsoft.Json;
using System;

[Serializable]
public class ZilRequest
{
    private const string Id = "1";
    private const string Jsonrpc = "2.0";

    public string id;
    public string jsonrpc;
    public string method;
    public object[] @params;

    public ZilRequest(string method, string @params)
    {
        id = Id;
        jsonrpc = Jsonrpc;
        this.method = method;
        this.@params = new string[] { @params };
    }

    public ZilRequest(string method, object[] @params)
    {
        id = Id;
        jsonrpc = Jsonrpc;
        this.method = method;
        this.@params = @params;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
}
