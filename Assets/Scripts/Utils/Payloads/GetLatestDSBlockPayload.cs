using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLatestDSBlockPayload : ResponsePayload
{
    public DSBlockHeader header;
    public string signature;

}
