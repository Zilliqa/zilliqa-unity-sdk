using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zilliqa.Core.Crypto;

namespace Zilliqa.Core.Crypto { 
    public class ScryptParams : KDFParams
    {
        public override string Salt { get; set; }
        public int DkLen { get; set; }
        public int N { get; set; }
        public int R { get; set; }
        public int P { get; set; }
    }
}

