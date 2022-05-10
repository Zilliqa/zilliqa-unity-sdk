using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusZil_Core.Utils
{
    public class HashUtil
    {
        public static byte[] CalculateSha256Hash(byte[] value)
        {
            var digest = new Sha256Digest();
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }

        public static byte[] HmacSha256(byte[] data, byte[] key)
        {
            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(key));
            var result = new byte[hmac.GetMacSize()];
            hmac.BlockUpdate(data, 0, data.Length);
            hmac.DoFinal(result, 0);
            return result;
        }
        public static byte[] CalculateKeccakHash(byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }
        public static byte[] GenerateMac(byte[] derivedKey, byte[] cipherText)
        {
            var result = new byte[16 + cipherText.Length];
            Array.Copy(derivedKey, 16, result, 0, 16);
            Array.Copy(cipherText, 0, result, 16, cipherText.Length);
            return CalculateKeccakHash(result);
        }
    }
}
