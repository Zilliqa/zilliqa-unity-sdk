using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResponsePayload {}

[Serializable]
public class CreateTransactionPayload : ResponsePayload
{
    [SerializeField]
    public string Info;
    [SerializeField]
    public string TranID;
}
