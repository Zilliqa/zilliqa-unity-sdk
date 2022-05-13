using System;
using Zilliqa.Wallet;
namespace Zilliqa.Requests
{
    [Serializable]
    public class GetBalanceResponse : ZilResponse
    {
        public Balance result;
    }
}