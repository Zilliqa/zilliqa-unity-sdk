using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zilliqa.Core.Crypto;
using Zilliqa.Utils;

namespace Zilliqa.Utils
{
    public class PBKDF2Params : KDFParams
    {
        public override string Salt { get; set; }
        public int DkLen { get; set; }
        public int Count { get; set; }
    }
}

