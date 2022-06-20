namespace Zilliqa.Requests
{
    public class GetRecentTransactionsPayload : ResponsePayload
    {
        public string[] TxnHashes;
        public int number;
    }
}
