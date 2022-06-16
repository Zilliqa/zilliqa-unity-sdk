using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zilliqa.Core.Beta;
using Zilliqa.Core.Crypto;
using Zilliqa.Utils;

namespace Zilliqa.Core
{
    public class Wallet
    {
        protected string privateKey;
        protected string publicKey;
        protected string address;
        protected string encryptedPk;
        protected byte[] encryptionKey;
        protected byte[] iv;
        public string PublicKey => publicKey;
        public string Address => address;
        public ECKeyPair ecKeyPair;

        public Wallet(string _privateKey)
        {
            privateKey = _privateKey;
            address = CryptoUtil.GetAddressFromPrivateKey(privateKey);
            publicKey = CryptoUtil.GetPublicKeyFromPrivateKey(privateKey, true);
            ecKeyPair = new ECKeyPair(new BigInteger(publicKey, 16), new BigInteger(privateKey, 16));
        }

      


        public bool Is(string otherAddress)
        {
            var addr = AddressUtils.AddPrefix_0x(Address.ToLower());
            var otherAddr = AddressUtils.AddPrefix_0x(otherAddress.ToLower());

            return addr.Equals(otherAddr);
        }

        public override string ToString()
        {
            return "publicKey: " + publicKey + " address: " + address + " privatekey: " + privateKey + " encrypted privatekey: " + encryptedPk;
        }


    }

    public class WalletKey
    {
        public string secretCipher;
        public string pkCipher;
        public KDFParams scrypt;
        public string secretMac;
        public string pkMac;
        public string iv;
        public string ivPk;

        public byte[] secretCipherBytes => ByteUtil.HexStringToByteArray(secretCipher);
        public byte[] pkCipherBytes => ByteUtil.HexStringToByteArray(pkCipher);
        public byte[] secretMacBytes => ByteUtil.HexStringToByteArray(secretMac);
        public byte[] pkMacBytes => ByteUtil.HexStringToByteArray(pkMac);
        public byte[] ivBytes => ByteUtil.HexStringToByteArray(iv);
        public byte[] ivPkBytes => ByteUtil.HexStringToByteArray(ivPk);

        public WalletKey(KDFParams _params)
        {
            scrypt = _params;
        }
    }
}