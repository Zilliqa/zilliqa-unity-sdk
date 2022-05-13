using System;
namespace Zilliqa.Requests
{
    public class GetDSBlockListingPayload : ResponsePayload
    {
        public DSBlock[] data;
    }

    [Serializable]
    public class DSBlock
    {
        public int BlockNum;
        public string Hash;
    }
}
