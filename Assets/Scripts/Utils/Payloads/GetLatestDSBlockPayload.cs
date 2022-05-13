namespace Zilliqa.Requests
{
    public class GetLatestDSBlockPayload : ResponsePayload
    {
        public DSBlockHeader header;
        public string signature;
    }
}
