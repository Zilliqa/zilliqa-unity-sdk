using System.Collections.Generic;
namespace Zilliqa.Requests
{
    public class GetTxBlockListingPayload
    {
        public List<TransactionBlock> data;
        public decimal maxPages;
    }
}
