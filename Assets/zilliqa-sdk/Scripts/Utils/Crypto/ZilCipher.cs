
using Zilliqa.Utils;

namespace Zilliqa.Core.Crypto
{
    public class ZilCipher
    {
        public string Cipher { get; set; }
        public CipherParams Cipherparams { get; set; }
        public string Ciphertext { get; set; }
        public string Kdf { get; set; }
        public KDFParams KdfParams { get; set; }
        public string Mac { get; set; }

    }

}
