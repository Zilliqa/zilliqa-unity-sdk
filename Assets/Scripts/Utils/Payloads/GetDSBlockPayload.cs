using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDSBlockPayload : ResponsePayload
{
    public DSBlockHeader header;
    public string signature;
}

[Serializable]
public class DSBlockHeader
{
      public string BlockNum;
      public int  Difficulty;
      public int DifficultyDS;
      public string GasPrice; 
      public string LeaderPubKey;
      public string[] PoWWinners;
      public string PrevHash; 
      public string Timestamp;
}
