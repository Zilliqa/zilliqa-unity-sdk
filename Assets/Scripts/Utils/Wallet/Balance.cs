using Zilliqa.Requests;
namespace Zilliqa.Wallet
{
    public class Balance : ResponsePayload
    {
        public decimal balance;
        public int nonce;
    }
}