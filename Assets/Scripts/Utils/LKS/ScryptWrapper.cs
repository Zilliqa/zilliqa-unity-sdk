using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ScryptWrapper
{
    public byte[] GetDerivedKey(
            byte[] password, byte[] salt, int n, int r, int p, int dkLen)
    {
        return SCrypt.Generate(password, salt, n, r, p, dkLen);
    }
}

