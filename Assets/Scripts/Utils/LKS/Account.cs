using MusZil_Core.Utils;
using Org.BouncyCastle.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Zilliqa.Testing
{
    public class Account : MonoBehaviour

    {
        private ECKeyPair keys;
        private string address;

        public ECKeyPair Keys
        {
            get { return keys; }
        }

        public string Address
        {
            get { return address; }
        }


        public Account(ECKeyPair keys)
        {
            this.keys = keys;
            this.address = KeyTools.GetAddressFromPublicKey(ByteUtil.ByteArrayToHexString(this.keys.publicKey.ToByteArray()));
        }

        public Account(string privateKey)
        {
            string publicKey = KeyTools.GetPublicKeyFromPrivateKey(privateKey, true);
            this.address = KeyTools.GetAddressFromPublicKey(publicKey);
            this.keys = new ECKeyPair(new BigInteger(privateKey, 16), new BigInteger(publicKey, 16));
        }

        public static Account FromFile(string file, string passphrase)
        {
            string privateKey = KeyTools.DecryptPrivateKey(file, passphrase);
            return new Account(privateKey);
        }

        public string ToFile(string privateKey, string passphrase, KDFType type)
        {
            return KeyTools.EencryptPrivateKey(privateKey, passphrase, type);
        }

        public string GetPublicKey()
        {
            return ByteUtil.ByteArrayToHexString(this.keys.publicKey.ToByteArray());
        }

        public string GetPrivateKey()
        {
            return ByteUtil.ByteArrayToHexString(this.keys.privateKey.ToByteArray());
        }

        public static string ToCheckSumAddress(string address)
        {
            if (!Validation.IsAddress(address))
            {
                throw new Exception("not a valid base 16 address");
            }

            address = address.ToLower().Replace("0x", "");
            string hash = ByteUtil.ByteArrayToHexString(HashUtil.Sha256(ByteUtil.HexStringToByteArray(address)));
            StringBuilder ret = new StringBuilder("0x");
            byte[] x = ByteUtil.HexStringToByteArray(hash);
            BigInteger v = new BigInteger(ByteUtil.HexStringToByteArray(hash));
            for (int i = 0; i < address.Length; i++)
            {
                if ("1234567890".IndexOf(address.ToCharArray()[i]) != -1)
                {
                    ret.Append(address.ToCharArray()[i]);
                }
                else
                {
                    BigInteger checker = v.And(BigInteger.ValueOf(2).Pow(255 - 6 * i));//(BigInteger.valueOf(2l).pow(255 - 6 * i))
                    ret.Append(checker.CompareTo(BigInteger.ValueOf(1)) < 0 ? address.ToCharArray()[i].ToString().ToLower() : address.ToCharArray()[i].ToString().ToUpper());
                }
            }
            return ret.ToString();
        }

        /*public static string NormaliseAddress(string address)
        {
            if (address == Contract.Contract.NIL_ADDRESS)
            {
                return address;
            }

            if (Validation.IsBech32(address))
            {
                return Bech32.FromBech32Address(address);
            }

            if (Validation.IsValidChecksumAddress(address))
            {
                return ToCheckSumAddress(address).Substring(2);
            }

            throw new Exception("Address format is invalid");
        }*/

    }

}