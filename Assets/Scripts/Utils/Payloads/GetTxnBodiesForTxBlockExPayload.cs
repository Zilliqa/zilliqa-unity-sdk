using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTxnBodiesForTxBlockExPayload : ResponsePayload
{
    public int CurrPage;
    public int NumPages;
    public ProcessedTransaction[] Transactions;
}
