using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRecentTransactionsPayload : ResponsePayload
{
    public string[] TxnHashes;
    public int number;
}
