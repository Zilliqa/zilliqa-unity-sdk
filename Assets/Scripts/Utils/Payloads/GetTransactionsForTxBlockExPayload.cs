namespace Zilliqa.Requests
{
    public class GetTransactionsForTxBlockExPayload : ResponsePayload
    {
        public int CurrPage;
        public int NumPages;
        public string[][] Transactions;
    }
}
