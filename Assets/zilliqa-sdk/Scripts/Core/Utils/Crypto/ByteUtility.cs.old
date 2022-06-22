using System;
using System.Text;

public class ByteUtility
{
    public static int GetHexVal(char hex)
    {
        int val = hex;
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }

    public static byte[] HexStringToByteArray(string hex)
    {
        if (hex.Length % 2 == 1)
            throw new Exception("The binary key cannot have an odd number of digits");

        byte[] arr = new byte[hex.Length >> 1];

        for (int i = 0; i < hex.Length >> 1; ++i)
        {
            arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
        }

        return arr;
    }

    public static string ByteArrayToHexString(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder(bytes.Length * 3);
        foreach (byte b in bytes)
        {
            sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
        }
        return sb.ToString().ToUpper().Replace(" ", "");
    }
}
