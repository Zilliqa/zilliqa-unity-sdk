namespace Zilliqa.Requests
{
    public class GetStateProofResponse : ZilResponse
    {
        public string[] accountProof;
        public string[] stateProof;
    }
}
