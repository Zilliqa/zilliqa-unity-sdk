using Zilliqa.Core;
using Org.BouncyCastle.Math;
using System;
using System.Text;
using Zilliqa.Utils;
namespace Zilliqa.Utils
{

    public class AddressUtils
    {
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
    }
}
