namespace Zilliqa.Requests
{
    public class GetBlockchainInfoPayload : ResponsePayload
    {
        public string CurrentDSEpoch;
        public string CurrentMiniEpoch;
        public float DSBlockRate;
        public string NumDSBlocks;
        public int NumPeers;
        public string NumTransactions;
        public string NumTxBlocks;
        public string NumTxnsDSEpoch;
        public string NumTxnsTxEpoch;
        public ShardingStructure ShardingStructure;
        public float TransactionRate;
        public float TxBlockRate;
    }

    public class ShardingStructure
    {
        public int[] NumPeers;

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < NumPeers.Length; i++)
            {
                str += NumPeers[i];
                if (i < NumPeers.Length - 1)
                    str += ",";
            }

            return str;
        }
    }
}
