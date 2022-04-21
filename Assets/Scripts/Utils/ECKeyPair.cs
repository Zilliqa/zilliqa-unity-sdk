using Org.BouncyCastle.Math;

public class ECKeyPair
{
    public BigInteger publicKey;
    public BigInteger privateKey;

    public ECKeyPair(BigInteger publicKey, BigInteger privateKey)
    {
        this.publicKey = publicKey;
        this.privateKey = privateKey;
    }
}
