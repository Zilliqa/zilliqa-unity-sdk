using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zilliqa.Requests
{
    public class GetSmartContractSubStateResponse : ZilResponse
    {
        //[JsonConverter(typeof(GetSmartContractSubStateConverter))]
        public WalletSellOrders result;
    }
}
