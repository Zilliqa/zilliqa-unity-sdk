using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zilliqa.Testing
{
    public class PBKDF2Params : KDFParams
    {
        public string Salt { get; set; }
        public int DkLen { get; set; }
        public int Count { get; set; }
    }
}

