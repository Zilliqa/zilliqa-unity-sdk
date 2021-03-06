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
    //Holds the credentials of a wallet
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

        /// <summary>
        /// Create a wallet with a given private key.
        /// </summary>
        /// <param name="_privateKey"></param>
        public Wallet(string _privateKey)
        {
            privateKey = _privateKey;
            address = CryptoUtil.GetAddressFromPrivateKey(privateKey);
            publicKey = CryptoUtil.GetPublicKeyFromPrivateKey(privateKey, true);
            ecKeyPair = new ECKeyPair(new BigInteger(publicKey, 16), new BigInteger(privateKey, 16));
        }

        /// <summary>
        /// Check if both wallets are the same.
        /// </summary>
        /// <param name="otherAddress"></param>
        /// <returns></returns>
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
}