using MusZil_Core.Utils;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zilliqa.Testing;
namespace Zilliqa.Testing
{
    public class KeyTools
    {
        public static ECDomainParameters CURVE;
        private static X9ECParameters CURVE_PARAMS = CustomNamedCurves.GetByName("secp256k1");
        private static KeyStore keystore = KeyStore.DefaultKeyStore();
        private static string pattern = "^(0x)?[0-9a-f]";


        static KeyTools()
        {
            CURVE = new ECDomainParameters(CURVE_PARAMS.Curve, CURVE_PARAMS.G, CURVE_PARAMS.N, CURVE_PARAMS.H);
        }

        public static string GeneratePrivateKey()
        {
            ECKeyPair keys = Schnorr.GenerateKeyPair();
            //return Numeric.ToHexStringNoPrefixZeroPadded(keys.privateKey, 64);
            return "";
        }

        [Obsolete]
        public static ECKeyPair GenerateKeyPair()
        {
            while (true)
            {
                ECKeyPair keyPair = Schnorr.GenerateKeyPair();
                if (keyPair.privateKey.ToString(16).Length == 64)
                {
                    return keyPair;
                }
            }
        }

        public static string GetAddressFromPrivateKey(string privateKey)
        {
            string publicKey = GetPublicKeyFromPrivateKey(privateKey, true);
            return GetAddressFromPublicKey(publicKey);
        }

        public static bool IsBytestring(string address)
        {
            System.Console.WriteLine(address);

            MatchCollection matchs = Regex.Matches(address, pattern);
            foreach (Match match in matchs)
            {
                System.Console.WriteLine(match.Groups);
            }
            return true;
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

        public static string GetAddressFromPublicKey(string publicKey)
        {
            byte[] address= new byte[0];// = GetAddressFromPublicKey(Numeric.HexStringToByteArray(publicKey));
            return ByteUtil.ByteArrayToHexString(address).Substring(24);
        }

        public static byte[] GetAddressFromPublicKey(byte[] publicKey)
        {
            return HashUtil.Sha256(publicKey);
        }

        public static byte[] GenerateRandomBytes(int size)
        {
            byte[] bytes = new byte[size];
            new SecureRandom().NextBytes(bytes);
            return bytes;
        }

        private static ECPoint GetPublicPointFromPrivate(BigInteger privateKeyPoint)
        {
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

        public static string EencryptPrivateKey(string privateKey, string passphrase, KDFType type)
        {
            return keystore.EncryptPrivateKey(privateKey, passphrase, type);
        }
    }

}