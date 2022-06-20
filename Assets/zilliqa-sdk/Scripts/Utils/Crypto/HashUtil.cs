using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Zilliqa.Core;
using Zilliqa.Core.Crypto;

namespace Zilliqa.Utils
{
    public class HashUtil
    {
        public static byte[] Sha256(byte[] bytes)
        {
            SHA256 sha = new SHA256Managed();
            return sha.ComputeHash(bytes);
        }
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

        public static byte[] GenerateAesCtrCipher(byte[] iv, byte[] encryptKey, byte[] input)
        {
            var key = ParameterUtilities.CreateKeyParameter("AES", encryptKey);

            var parametersWithIv = new ParametersWithIV(key, iv);

            var cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            cipher.Init(true, parametersWithIv);
            return cipher.DoFinal(input);
        }

        public static byte[] GetDerivedKey(byte[] password, KDFParams parameters)
        {
            if (parameters is Pbkdf2)
            {
                var pbkdf2Params = (Pbkdf2)parameters;
                return pbkdf2Params.GetDerivedKey(password);
            }
            else if (parameters is KDFParams)
            {
                return GenerateDerivedScryptKey(
                    Encoding.UTF8.GetString(password),
                    ByteUtil.HexStringToByteArray(parameters.Salt),
                    parameters.n,
                    parameters.r,
                    parameters.p,
                    parameters.dklen);
            }
            else
            {
                throw new Exception("unsupported kdf params");
            }
        }

        private static byte[] GenerateDerivedScryptKey(string pwd)
        {
            //Using Scrypt.Net from nuget
            var encoder = new Scrypt.ScryptEncoder();
            //ScyptDec
            var key = encoder.Encode(pwd);

            return Encoding.UTF8.GetBytes(key);
        }

        private static byte[] GenerateDerivedScryptKey(string pwd, byte[] saltBytes, int iterationCount, int blockSize, int threadCount, int DKLength)
        {
            //Using Scrypt.Net from nuget
            var encoder = new Scrypt.ScryptEncoder();
            //ScyptDec
            var key = encoder.Encode(pwd, saltBytes, iterationCount, blockSize, threadCount, DKLength);

            return Encoding.UTF8.GetBytes(key);
        }


    }
}
