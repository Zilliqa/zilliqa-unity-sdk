using System;

namespace Zilliqa.Requests
{
    [Serializable]
    public class GetTxBlockListingResponse : ZilResponse
    {
        public GetTxBlockListingPayload result;
    }
}
