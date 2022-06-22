using Zilliqa.Core;
using Org.BouncyCastle.Math;
using System;
using System.Text;

namespace Zilliqa.Utils
{
    /// <summary>
    /// Holds utility functionalities related to block chain addresses.
    /// </summary>
    public class AddressUtils
    {
        /// <summary>
        /// Returns the checksummed version of a given address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string ToCheckSumAddress(string address)
        {
            if (!Validation.IsAddress(address))
            {
                throw new Exception("not a valid base 16 address");
            }
            address = address.ToLower().Replace("0x", "");
            string hash = ByteUtil.ByteArrayToHexString(HashUtil.Sha256(ByteUtil.HexStringToByteArray(address)));
            StringBuilder ret = new StringBuilder("0x");
            BigInteger v = new BigInteger(ByteUtil.HexStringToByteArray(hash));
            for (int i = 0; i < address.Length; i++)
            {
                if ("1234567890".IndexOf(address.ToCharArray()[i]) != -1)
                {
                    ret.Append(address.ToCharArray()[i]);
                }
                else
                {
                    BigInteger checker = v.And(BigInteger.ValueOf(2).Pow(255 - 6 * i));
                    ret.Append(checker.CompareTo(BigInteger.ValueOf(1)) < 0 ? address.ToCharArray()[i].ToString().ToLower() : address.ToCharArray()[i].ToString().ToUpper());
                }
            }
            return ret.ToString();
        }

        /// <summary>
        /// Returns the address with the "0x". 
        /// If the given address already has the prefix, the same value will be returned.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>The address with the "0x" prefix</returns>
        public static string AddPrefix_0x(string address)
        {
            if (!address.Substring(0, 2).Equals("0x"))
                return "0x" + address;

            return address;
        }

        /// <summary>
        /// Returns the address without the "0x" prefix,=.
        /// If the given address doesn't have the prefix, the same value will be returned.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>The address with the "0x" prefix</returns>
        public static string RemovePrefix_0x(string address)
        {
            if (address.Substring(0, 2).Equals("0x"))
                return address.Substring(2, address.Length - 2);

            return address;
        }
    }
}
