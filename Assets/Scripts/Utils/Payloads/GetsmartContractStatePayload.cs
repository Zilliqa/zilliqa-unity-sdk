using System.Collections.Generic;
namespace Zilliqa.Requests
{
    public class GetsmartContractStatePayload
    {
        public decimal _balance;
        public string admin;
        public Dictionary<string, string> balances { get; set; } = new();
        public Dictionary<string, Dictionary<string, string>> allowances { get; set; } = new();
        public string implementation;
        public string total_supply;
    }
}
