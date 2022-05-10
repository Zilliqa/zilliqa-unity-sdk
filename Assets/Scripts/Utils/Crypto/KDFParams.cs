using Newtonsoft.Json;

namespace MusZil_Core.Crypto
{
    public class KDFParams
    {
        public KDFParams()
        {
        }

        [JsonProperty("salt")]
        public virtual string Salt { get; set; }
        public int n { get; set; } = 8192;
        public int c { get; set; } = 262144;
        public int r { get; set; } = 8;
        public int p { get; set; } = 1;
        public int dklen { get; set; } = 32;

    }

    public class PBKDF2Params : KDFParams
    {
        public override string Salt { get; set; }
        public int DkLen { get; set; }
        public int Count { get; set; }
    }


}
