using MusZil_Core.Utils;
using Org.BouncyCastle.Math;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Checksum16
{
    public const int ACC_ADDR_SIZE = 42;
   public static string Checksum(string origAddress)
    {
        if (!(origAddress.Length != ACC_ADDR_SIZE * 2 + 2) &&
        !(origAddress.Length != ACC_ADDR_SIZE * 2))
        {
            Debug.LogWarning("Size inappropriate");
            return "";
        }

        if (origAddress.Length == ACC_ADDR_SIZE * 2 + 2)
        {
            if (origAddress.Substring(0, 2) != "0x")
            {
                return "";
            }

            origAddress = origAddress.Remove(0, 2);
        }

        string lower_case_address = origAddress.ToLower();

        byte[] tmpaddr = ByteUtility.HexStringToByteArray(lower_case_address);


        byte[] hash_s = HashUtil.CalculateSha256Hash(tmpaddr);

        BigInteger temp_1 = new BigInteger("1");
        string ret = "";

        string hash_str = ByteUtility.ByteArrayToHexString(hash_s);

        BigInteger v = new BigInteger("0x" + hash_str, 16);
        

        for (int i = 0; i < lower_case_address.Length; i++)
        {
            if (lower_case_address[i] >= '0' && lower_case_address[i] <= '9')
            {
                ret += lower_case_address[i];
            }
            else
            {
                if (v.And(temp_1.ShiftLeft(255 - 6 * i)).Equals(1))
                {
                    ret += lower_case_address[i].ToString().ToUpper();
                }
                else
                {
                    ret += lower_case_address[i];
                }
            }
        }

        return ret;
    }
}
