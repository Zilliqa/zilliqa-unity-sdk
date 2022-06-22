using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;


namespace Zilliqa.Requests
{
    public class GetSmartContractSubStateResponse<T> : ZilResponse
    {
        //[JsonConverter(typeof(GetSmartContractSubStateConverter))]
        public Dictionary<string, T> result;
    }
}
