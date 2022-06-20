using Zilliqa.Core.Crypto;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using ECPoint = Org.BouncyCastle.Math.EC.ECPoint;
using Zilliqa.Core;
using System.Threading.Tasks;

namespace Zilliqa.Utils
{
    public class CryptoUtil
    {
        

        #region Keystore utils
        /**
         * The parameters of the secp256k1 curve that Bitcoin uses.
         */
        
        private static KeyStore keystore = new KeyStore();
        private static string pattern = "^(0x)?[0-9a-f]";


        public static string GeneratePrivateKey()
        {
            Zilliqa.Core.Crypto.ECKeyPair keys = Schnorr.GenerateKeyPair();
            return keys.PrivateKey.ToString(8);
        }

        public static string GetAddressFromPrivateKey(string privateKey)
        { 
            string publicKey = GetPublicKeyFromPrivateKey(privateKey, true);
            return GetAddressFromPublicKey(publicKey);
        }

        public static bool IsBytestring(string address)
        {
            var match = Regex.Match(address, pattern);
            
            return match.Success;
        }

        /**
         * @param privateKey hex string without 0x
         * @return
         */
        public static string GetPublicKeyFromPrivateKey(string privateKey, bool compressed)
        {
            BigInteger bigInteger = new BigInteger(privateKey, 16);
            ECPoint point = GetPublicPointFromPrivate(bigInteger);
            return ByteUtil.ByteArrayToHexString(point.GetEncoded(compressed));
        }

        public static string GetPublicKeyFromPrivateKey(BigInteger privateKey, bool compressed)
        {
            BigInteger bigInteger = privateKey;
            ECPoint point = GetPublicPointFromPrivate(bigInteger);
            return ByteUtil.ByteArrayToHexString(point.GetEncoded(compressed));
        }

        public static string GetAddressFromPublicKey(string publicKey)
        {
            SHA256 s = new SHA256Managed();
            byte[] address = s.ComputeHash(ByteUtil.HexStringToByteArray(publicKey));
            return ByteUtil.ByteArrayToHexString(address).Substring(24);
        }

        public static byte[] GenerateRandomBytes(int size)
        {
            byte[] bytes = new byte[size];
            new SecureRandom().NextBytes(bytes);
            return bytes;
        }

        private static ECPoint GetPublicPointFromPrivate(BigInteger privateKeyPoint)
        {
            var CURVE_PARAMS = CustomNamedCurves.GetByName("secp256k1");
            var CURVE = new ECDomainParameters(CURVE_PARAMS.Curve, CURVE_PARAMS.G, CURVE_PARAMS.N, CURVE_PARAMS.H);
            if (privateKeyPoint.BitLength > CURVE.N.BitLength)
            {
                privateKeyPoint = privateKeyPoint.Mod(CURVE.N);
            }
            return new FixedPointCombMultiplier().Multiply(CURVE.G, privateKeyPoint);
        }

        public static string DecryptPrivateKey(string file, string passphrase)
        {
            return keystore.DecryptPrivateKey(file, passphrase);
        }

        public static string EncryptPrivateKey(string privateKey, string passphrase, KDFType type)
        {
            return keystore.EncryptPrivateKey(privateKey, passphrase, type);
        }

        public static byte[] GenerateAesCtrCipher(byte[] iv, byte[] encryptKey, byte[] input)
        {
            var key = ParameterUtilities.CreateKeyParameter("AES", encryptKey);

            var parametersWithIv = new ParametersWithIV(key, iv);

            var cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            cipher.Init(true, parametersWithIv);
            return cipher.DoFinal(input);
        }
        #endregion

        
    }
}
