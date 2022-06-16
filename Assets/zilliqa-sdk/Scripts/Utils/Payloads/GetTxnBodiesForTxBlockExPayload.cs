using Zilliqa.Utils;

namespace Zilliqa.Requests
{
    public class GetTxnBodiesForTxBlockExPayload : ResponsePayload
    {
        public int CurrPage;
        public int NumPages;
        public ProcessedTransaction[] Transactions;
    }
}
