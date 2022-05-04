using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
