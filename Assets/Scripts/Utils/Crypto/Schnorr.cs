using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusZil_Core.Crypto
{
    public class Schnorr
    {
       

            static private X9ECParameters secp256k1 = ECNamedCurveTable.GetByName("secp256k1");

            static private int PUBKEY_COMPRESSED_SIZE_BYTES = 33;

            static private byte[] ALG = System.Text.Encoding.Default.GetBytes("Schnorr+SHA256 ");

            static private int ENT_BITS = 256; // 32 bytes of entropy require for the k value

            static readonly SecureRandom secureRandom = new SecureRandom();

            public static ECKeyPair GenerateKeyPair()
            {
                var gen = new ECKeyPairGenerator("EC");
                var keyGenParam = new KeyGenerationParameters(secureRandom, 256);
                gen.Init(keyGenParam);
                var keyPair = gen.GenerateKeyPair();
                var d = ((ECPrivateKeyParameters)keyPair.Private).D;
                var q = ((ECPublicKeyParameters)keyPair.Public).Q;

                return new ECKeyPair(d, new BigInteger(1, q.GetEncoded(true)));
            }


            public static Signature Sign(ECKeyPair kp, byte[] message)
            {
                SecureRandom drbg = GetDRBG(message);
                int len = secp256k1.N.BitLength / 8;
                byte[] bytes = new byte[len];
                drbg.NextBytes(bytes);

                Signature signature = null;
                while (signature == null)
                {
                    BigInteger k = new BigInteger(1, bytes);
                    signature = TrySign(kp, message, k);
                }

                return signature;
            }

            public static Signature TrySign(ECKeyPair kp, byte[] msg, BigInteger k)
            {
                BigInteger n = secp256k1.N;
                BigInteger privateKey = kp.PrivateKey;
                ECPoint publicKey = secp256k1.Curve.DecodePoint(kp.PublicKey.ToByteArray());

                // 1a. check if private key is 0
                if (privateKey == (BigInteger.Zero))
                {
                    throw new Exception("Private key must be >= 0");
                }

                // 1b. check if private key is less than curve order, i.e., within [1...n-1]
                if (privateKey.CompareTo(n) >= 0)
                {
                    throw new Exception("Private key cannot be greater than curve order");
                }

                // 2. Compute commitment Q = kG, where G is the base point
                ECPoint Q = secp256k1.G.Multiply(k);

                // 3. Compute the challenge r = H(Q || pubKey || msg)
                // mod reduce r by the order of secp256k1, n
                BigInteger r = Hash(Q, publicKey, msg).Mod(secp256k1.N);

                if (r == (BigInteger.Zero))
                {
                    return null;
                }

                //4. Compute s = k - r * prv
                // 4a. Compute r * prv
                BigInteger s = r.Multiply(privateKey).Mod(n);
                // 4b. Compute s = k - r * prv mod n
                s = k.Subtract(s).Mod(n);

                if (s == (BigInteger.Zero))
                {
                    return null;
                }

                return new Signature() { R = r, S = s };
            }

            static private BigInteger Hash(ECPoint q, ECPoint pubKey, byte[] msg)
            {
                // 33 q + 33 pubKey + variable msgLen
                int totalLength = PUBKEY_COMPRESSED_SIZE_BYTES * 2 + msg.Length;
                byte[] qCompressed = q.GetEncoded(true);
                byte[] pubKeyCompressed = pubKey.GetEncoded(true);
                byte[] hashInput = new byte[totalLength];

                Arrays.Fill(hashInput, (byte)0);
                Array.Copy(qCompressed, 0, hashInput, 0, PUBKEY_COMPRESSED_SIZE_BYTES);
                Array.Copy(pubKeyCompressed, 0, hashInput, PUBKEY_COMPRESSED_SIZE_BYTES, PUBKEY_COMPRESSED_SIZE_BYTES);
                Array.Copy(msg, 0, hashInput, PUBKEY_COMPRESSED_SIZE_BYTES * 2, msg.Length);

                System.Security.Cryptography.SHA256 sha = new System.Security.Cryptography.SHA256Managed();
            
                byte[] hash = sha.ComputeHash(hashInput);

                return new BigInteger(1, hash);
            }

            public static bool Verify(byte[] msg, Signature sig, ECPoint publicKey)
            {
                if (sig.R == BigInteger.Zero || sig.S == BigInteger.Zero)
                {
                    throw new Exception("Invalid R or S value: cannot be zero.");
                }

                if (sig.R.SignValue == -1 || sig.S.SignValue == -1)
                {
                    throw new Exception("Invalid R or S value: cannot be negative.");
                }

                if (publicKey.Curve != (secp256k1.Curve))
                {
                    throw new Exception("The public key must be a point on secp256k1.");
                }

                if (!publicKey.IsValid())
                {
                    throw new Exception("Invalid public key.");
                }

                ECPoint l = publicKey.Multiply(sig.R);
                ECPoint r = secp256k1.G.Multiply(sig.S);
                ECPoint Q = l.Add(r);

                if (Q.IsInfinity || !Q.IsValid())
                {
                    throw new Exception("Invalid intermediate point.");
                }

                BigInteger r1 = Hash(Q, publicKey, msg).Mod(secp256k1.N);

                if (r1 == (BigInteger.Zero))
                {
                    throw new Exception("Invalid hash.");
                }

                return r1.Equals(sig.R);
            }

            private static SecureRandom GetDRBG(byte[] message)
            {
                Sha256Digest sha256 = new Sha256Digest();
                HMac hMac = new HMac(sha256);
                return new SP800SecureRandomBuilder()
                        .SetEntropyBitsRequired(ENT_BITS)
                        .SetPersonalizationString(ALG)
                        .BuildHMac(hMac, message, true);
            }
        
    }
}
