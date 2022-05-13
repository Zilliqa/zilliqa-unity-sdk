using System;
using System.Collections.Generic;
using System.Text;

namespace Zilliqa.Core
{
    public class Bech32
    {
        public string hrp;
        public byte[] data;
        protected string _address;

        public Bech32(string hrp, byte[] data)
        {
            this.hrp = hrp;
            this.data = data;
            _address = "";
        }
        public Bech32(string address, byte[] data,string hrp = "zil")
        {
            if (!address.StartsWith(hrp))
                throw new ArgumentException("HRP is not zil");

            this.hrp = hrp;
            this.data = data;
            this._address = address;
        }

        public string ToCheckSumAddress(string address)
        {
            string result = string.Empty;
            string lower = address.ToLower();
            lower=lower.Replace("0x", "");
            return result;
        }

        public override string ToString()
        {
            return _address;
        }
    }
}
