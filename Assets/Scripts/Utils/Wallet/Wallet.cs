using Org.BouncyCastle.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zilliqa.Core.Crypto;
using Zilliqa.Utils;

namespace Zilliqa.Core
{
    public class Wallet
    {
        private string privateKey;
        private string publicKey;
        private string address;

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
    }
}