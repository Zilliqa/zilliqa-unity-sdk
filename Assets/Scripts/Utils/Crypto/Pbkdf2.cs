using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusZil_Core.Crypto
{
    public class Pbkdf2 : KDFParams
    {
        public override string Salt { get; set; }
        public int DkLen { get; set; }
        public int Count { get; set; }
        public Pbkdf2(string salt, int count, int dklen = 32)
        {
            Salt = salt;
            Count = count;
            DkLen = dklen;
        }
        public Pbkdf2(byte[] salt, int count, int dklen = 32)
        {
            Salt = Encoding.UTF8.GetString(salt);
            Count = count;
            DkLen = dklen;
        }
        public byte[] GetDerivedKey(byte[] password)
        {
            Pkcs5S2ParametersGenerator generator = new Pkcs5S2ParametersGenerator(new Sha256Digest());
            generator.Init(password, Encoding.UTF8.GetBytes(Salt), Count);
            return ((KeyParameter)generator.GenerateDerivedParameters("pbkdf2", DkLen * 8)).GetKey();
        }
    }
}
