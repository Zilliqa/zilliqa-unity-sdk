using System;
namespace Zilliqa.Requests
{
    public class GetMinerInfoPayload : ResponsePayload
    {
        public string[] dscommittee;
        public Shard[] shards;
    }

    [Serializable]
    public class Shard
    {
        public string[] nodes;
        public int size;
    }
}
