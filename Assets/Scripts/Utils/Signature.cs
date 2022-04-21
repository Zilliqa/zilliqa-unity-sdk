using Org.BouncyCastle.Math;

public class Signature
{
    public BigInteger R { get; set; }
    public BigInteger S { get; set; }

    public override string ToString()
    {
        string rHex = ByteUtility.ByteArrayToHexString(R.ToByteArray());
        while (rHex.Length < 64)
        {
            rHex = "0" + rHex;
        }

        while (rHex.Length > 64 && rHex.StartsWith("0"))
        {
            rHex = rHex.Substring(1);
        }

        string sHex = ByteUtility.ByteArrayToHexString(S.ToByteArray());
        while (sHex.Length < 64)
        {
            sHex = "0" + sHex;
        }

        while (sHex.Length > 64 && sHex.StartsWith("0"))
        {
            sHex = sHex.Substring(1);
        }

        return rHex + sHex;
    }
}
