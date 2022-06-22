namespace Zilliqa.Requests
{
    public class GetTransactionStatusResponse : ZilResponse
    {
        public GetTransactionStatusPayload result;

        public override string ToString()
            => "{\n\t<b>Id</b>: " + result.Id + "\n" +
                "\t<b>Oid</b>: " + result.Oid.ToString() + "\n" +
                "\t<b>Amount</b>: " + result.Amount + "\n" +
                "\t<b>Data</b>: " + result.Data + "\n" +
                "\t<b>EpochInserted</b>: " + result.EpochInserted + "\n" +
                "\t<b>EpochUpdated</b>: " + result.EpochUpdated + "\n" +
                "\t<b>GasLimit</b>: " + result.GasLimit + "\n" +
                "\t<b>GasPrice</b>: " + result.GasPrice + "\n" +
                "\t<b>LastModified</b>: " + result.LastModified + "\n" +
                "\t<b>ModificationState</b>: " + "<color=#39002e>" + result.ModificationState + "</color>\n" +
                "\t<b>Nonce</b>: " + result.Nonce + "\n" +
                "\t<b>SenderAddr</b>: " + result.SenderAddr + "\n" +
                "\t<b>Signature</b>: " + result.Signature + "\n" +
                "\t<b>Status</b>: " + "<color=#39002e>" + result.Status + "</color>\n" +
                "\t<b>Success</b>: " + "<color=#39000d>" + result.Success + "</color>\n" +
                "\t<b>ToAddr</b>: " + result.ToAddr + "\n" +
                "\t<b>Version</b>: " + result.Version + "\n}";


    }
}
