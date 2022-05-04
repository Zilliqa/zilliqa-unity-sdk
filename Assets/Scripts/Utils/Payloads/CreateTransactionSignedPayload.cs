using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResponsePayload {}

[Serializable]
public class CreateTransactionSignedPayload : ResponsePayload
{
    [SerializeField]
    public string Info;
    [SerializeField]
    public string TranID;
}
