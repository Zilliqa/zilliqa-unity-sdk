using Zilliqa.Core;
namespace Zilliqa.Requests
{
    public class GetTxBlockPayload : ResponsePayload
    {
        public BlockBody body;
        public TxHeader header;
    }
}
