using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Zilliqa.Utils
{
    public class Validation
    {

        //using low case
        public static bool IsByteString(string str, int len)
        {
            return Regex.IsMatch(str, "^(0x)?[0-9a-fA-F]{" + len + "}");
        }

        public static bool IsBech32(string str)
        {
            return Regex.IsMatch(str, "^zil1[qpzry9x8gf2tvdw0s3jn54khce6mua7l]{38}");
        }
        public static bool IsAddress(string address)
        {
            return IsByteString(address, 40);
        }

        public static bool IsPublicKey(string publicKey)
        {
            return IsByteString(publicKey, 66);
        }

        public static bool IsPrivateKey(string privateKey)
        {
            return IsByteString(privateKey, 64);
        }

        public static bool IsSignature(string signature)
        {
            return IsByteString(signature, 128);
        }


        public static bool IsValidChecksumAddress(string address)
        {
            return IsAddress(address.Replace("0x", "")) && AddressUtils.ToCheckSumAddress(address) == address;
        }


        public static string IntToHex(int value, int size)
        {
            string hexVal = value.ToString("X");
            char[] hexRep = new char[hexVal.Length];
            for (int i = 0; i < hexVal.Length; i++)
            {
                hexRep[i] = hexVal.ToCharArray()[i];
            }

            List<char> hex = new List<char>();

            for (int i = 0; i < size - hexVal.Length; i++)
            {
                hex.Add('0');
            }

            for (int i = 0; i < hexVal.Length; i++)
            {
                hex.Add(hexRep[i]);
            }

            StringBuilder builder = new StringBuilder();
            foreach (char c in hex)
            {
                builder.Append(c);
            }
            return builder.ToString();

        }

    }
}