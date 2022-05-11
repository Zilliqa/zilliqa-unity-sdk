using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Zilliqa.Testing
{
    public class ScryptParams : KDFParams
    {
        public string Salt { get; set; }
        public int DkLen { get; set; }
        public int N { get; set; }
        public int R { get; set; }
        public int P { get; set; }
    }
}

