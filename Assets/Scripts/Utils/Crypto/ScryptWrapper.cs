using Org.BouncyCastle.Crypto.Generators;

namespace Zilliqa.Core.Crypto
{
    public class ScryptWrapper
    {
        public byte[] GetDerivedKey(
                byte[] password, byte[] salt, int n, int r, int p, int dkLen)
        {
            return SCrypt.Generate(password, salt, n, r, p, dkLen);
        }
    }
}

