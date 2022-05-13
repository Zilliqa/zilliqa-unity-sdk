using System;
namespace Zilliqa.Requests
{
    [Serializable]
    public class CreateTransactionResponse : ZilResponse
    {
        public CreateTransactionPayload result;
    }
}

