using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using System;
using System.Linq;

namespace MusZil_Core.Crypto
{
    public class ECKeyPair
    {
        static X9ECParameters CURVE_PARAMS = CustomNamedCurves.GetByName("secp256k1");
        static ECDomainParameters CURVE = new ECDomainParameters(
                CURVE_PARAMS.Curve, CURVE_PARAMS.G, CURVE_PARAMS.N, CURVE_PARAMS.H);

        private BigInteger privateKey;
        private BigInteger publicKey;

        public ECKeyPair(BigInteger privateKey, BigInteger publicKey)
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
