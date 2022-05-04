using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTransactionsForTxBlockExPayload : ResponsePayload
{
    public int CurrPage;
    public int NumPages;
    public string[][] Transactions;
}
