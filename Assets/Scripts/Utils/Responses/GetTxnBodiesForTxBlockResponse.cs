using Zilliqa.Utils;
namespace Zilliqa.Requests
{
    public class GetTxnBodiesForTxBlockResponse : ZilResponse
    {
        public ProcessedTransaction[] result;
    }
}
