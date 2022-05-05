using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTxBlockPayload : ResponsePayload
{
    public BlockBody body;
    public TxHeader header;
}
