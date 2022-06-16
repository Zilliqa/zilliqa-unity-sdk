using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace Zilliqa.Core
{
    public class PBKDF2Wrapper
    {
        public byte[] GetDerivedKey(byte[] password, byte[] salt, int iterationCount, int keySize)
        {
            Pkcs5S2ParametersGenerator generator = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            generator.Init(password, salt, iterationCount);
#pragma warning disable CS0618 // Type or member is obsolete
            return ((KeyParameter)generator.GenerateDerivedParameters(keySize * 8)).GetKey();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
