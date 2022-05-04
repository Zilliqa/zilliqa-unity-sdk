using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
