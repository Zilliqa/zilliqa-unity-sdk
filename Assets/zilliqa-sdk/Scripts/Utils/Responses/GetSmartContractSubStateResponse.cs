using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Zilliqa.Marketplace;

namespace Zilliqa.Requests
{
    public class GetSmartContractSubStateResponse : ZilResponse
    {
        //[JsonConverter(typeof(GetSmartContractSubStateConverter))]
        public WalletOrders result;
    }
}
