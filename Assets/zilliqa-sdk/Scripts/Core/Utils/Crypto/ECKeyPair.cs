using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Linq;

namespace Zilliqa.Core.Crypto
{
    /// <summary>
    /// You can use this class to hold a private/public key pair, or generate a new one.
    /// </summary>
    public class ECKeyPair
    {
        static X9ECParameters CURVE_PARAMS = CustomNamedCurves.GetByName("secp256k1");
        static ECDomainParameters CURVE = new ECDomainParameters(
                CURVE_PARAMS.Curve, CURVE_PARAMS.G, CURVE_PARAMS.N, CURVE_PARAMS.H);
        static readonly SecureRandom secureRandom = new SecureRandom();
        public BigInteger privateKey;
        public BigInteger publicKey;

        public ECKeyPair(BigInteger publicKey, BigInteger privateKey)
        {
            this.privateKey = privateKey;
            this.publicKey = publicKey;
        }

        public BigInteger PrivateKey
        {
            get
            {
                return privateKey;
            }
        }

        public BigInteger PublicKey
        {
            get
            {
                return publicKey;
            }
        }

        public static ECKeyPair Create(BigInteger privateKey)
        {
            return new ECKeyPair(privateKey, PublicKeyFromPrivate(privateKey));
        }
        public static ECKeyPair GenerateKeyPair()
        {
            byte[] pkArray = new byte[0];
            BigInteger D = new BigInteger("0");
            ECPoint Q = null;
            int iterationCount = 0;
            while (pkArray.Length != 32)
            {
                iterationCount++;

                var gen = new ECKeyPairGenerator();
                var keyGenParam = new KeyGenerationParameters(new SecureRandom(), 256);
                gen.Init(keyGenParam);
                var keyPair = gen.GenerateKeyPair();
                D = ((ECPrivateKeyParameters)keyPair.Private).D;
                Q = ((ECPublicKeyParameters)keyPair.Public).Q;

                pkArray = BigIntegers.AsUnsignedByteArray(D);

            }


            return new ECKeyPair(D, new BigInteger(1, Q.GetEncoded(true)));
        }

        public static ECKeyPair Create(byte[] privateKey)
        {
            return Create(new BigInteger(privateKey));
        }

        public static BigInteger PublicKeyFromPrivate(BigInteger privKey)
        {
            ECPoint point = publicPointFromPrivate(privKey);

            byte[] encoded = point.GetEncoded(false);
            return new BigInteger(1, encoded.Skip(1).ToArray());  // remove prefix
        }

        public static ECPoint publicPointFromPrivate(BigInteger privKey)
        {
            if (privKey.CompareTo(CURVE.N) != -1 || privKey.Equals(BigInteger.Zero))
            {
                throw new Exception("Invalid private key.");
            }
            return new FixedPointCombMultiplier().Multiply(CURVE.G, privKey);
        }
    }

}
