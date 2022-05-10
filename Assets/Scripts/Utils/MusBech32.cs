using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MusZil_Core.Utils
{
    public class MusBech32
    {
        private string _hrp;
        private string[] INVALID_DATA;

        private const int DMIN_LENGTH = 6;
        private const int DMAX_LENGTH = 6;
        private const string SEPARATOR = "1";
        public const string HRP = "zil";

        /**
         * The Bech32 character set for encoding.
         */
        private static string CHARSET = "qpzry9x8gf2tvdw0s3jn54khce6mua7l";

        /**
         * The Bech32 character set for decoding.
         */
        private static sbyte[] CHARSET_REV = {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            15, -1, 10, 17, 21, 20, 26, 30, 7, 5, -1, -1, -1, -1, -1, -1,
            -1, 29, -1, 24, 13, 25, 9, 8, 23, -1, 18, 22, 31, 27, 19, -1,
            1, 0, 3, 16, 11, 28, 12, 14, 6, 4, 2, -1, -1, -1, -1, -1,
            -1, 29, -1, 24, 13, 25, 9, 8, 23, -1, 18, 22, 31, 27, 19, -1,
            1, 0, 3, 16, 11, 28, 12, 14, 6, 4, 2, -1, -1, -1, -1, -1 };


        #region public methods

        public static string Bech32ToBase16Address(string address)
        {
            var res = "0x";
            bool lower = false, upper = false;

            Bech32 data = Decode(address);
            for (int i = 0; i < address.Length; ++i)
            {
                char c = address.ToCharArray()[i];
                if (c < 33 || c > 126) throw new ArgumentException($"Invalid character {c}");
                if (c >= 'a' && c <= 'z')
                {
                    if (upper)
                        throw new ArgumentException($"Invalid character {c}");
                    lower = true;
                }
                if (c >= 'A' && c <= 'Z')
                {
                    if (lower)
                        throw new ArgumentException($"Invalid character {c}");
                    upper = true;
                }
            }
            if (data.hrp != HRP)
            {
                throw new Exception("Expected hrp to be zil");
            }

            List<int> bits = ByteUtil.ConvertBits(data.data, 5, 8, false);
            byte[] buf = new byte[bits.Count];
            for (int i = 0; i < bits.Count; i++)
            {
                buf[i] = BitConverter.GetBytes(bits[i])[0];
            }
            if (null == buf || buf.Length == 0)
            {
                throw new Exception("Could not convert buffer to bytes");
            }
            res += ByteUtil.ByteArrayToHexString(buf);
            return res;
        }

        public static string Base16ToBech32Address(string address)
        {

            address = address.ToLower().Replace("0x", "");

            List<int> bits = ByteUtil.ConvertBits(ByteUtil.HexStringToByteArray(address), 8, 5, false);

            byte[] addrBz = new byte[bits.Count];

            for (int i = 0; i < bits.Count; i++)
            {
                addrBz[i] = BitConverter.GetBytes(bits[i])[0];
            }

            var res = Encode(HRP, addrBz);
            return res.Substring(0,42);
        }

        #endregion

        #region Private methods

        #region Encode/Decode

        public static Bech32 Decode(string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                char c = str.ToCharArray()[i];
                if (c < 33 || c > 126) throw new ArgumentException($"Invalid character {c}");
            }
            int pos = str.LastIndexOf('1');
            if (pos < 1) throw new ArgumentException("Missing human-readable part");
            int dataPartLength = str.Length - 1 - pos;
            if (dataPartLength < 6)
                throw new ArgumentException("Data part too short!");
            byte[] values = new byte[dataPartLength];
            for (int i = 0; i < dataPartLength; ++i)
            {
                char c = str.ToCharArray()[(i + pos + 1)];
                if (CHARSET_REV[c] == -1) throw new InvalidEnumArgumentException($"Invalid character {c} pos: {i + pos + 1}");
                values[i] = (byte)CHARSET_REV[c];
            }
            string hrp = str.Substring(0, pos).ToLower(CultureInfo.CurrentCulture);
            if (!VerifyChecksum(hrp, values)) throw new InvalidOperationException("Wrong Checksum");
            byte[] newValues = new byte[values.Length - 6];
            Array.Copy(values, newValues, values.Length - 6);
            return new Bech32(str, newValues, HRP);
        }



        public static string Encode(Bech32 bech32,int length = 42)
        {
            return Encode(bech32.hrp, bech32.data).Substring(0,length);
        }
        public static string Encode(string hrp, byte[] values)
        {
            if (hrp.Length < 1)
                throw new Exception("Human-readable part is too short");
            if (hrp.Length > 83)
                throw new Exception("Human-readable part is too long");

            hrp = hrp.ToLower();
            byte[] checksum = CreateChecksum(hrp, values);
            byte[] combined = new byte[values.Length + checksum.Length + 1];
            Array.Copy(values, 0, combined, 0, values.Length);
            Array.Copy(checksum, 0, combined, values.Length, checksum.Length);
            StringBuilder sb = new StringBuilder(hrp.Length + 1 + combined.Length);
            sb.Append(hrp);
            sb.Append('1');
            foreach (byte b in combined)
            {
                sb.Append(CHARSET.ToCharArray()[b]);
            }
            sb.Insert(sb.Length, ByteUtil.ByteArrayToHexString(values));
            return sb.ToString();
        }

        public static string Encode(string hrp, string address)
        {
            return Encode(hrp, ByteUtil.HexStringToByteArray(address));
        }

        #endregion

        #region Checksum
        private static byte[] CreateChecksum(string hrp, byte[] data)
        {
            byte[] hrpExpanded = GetBytesFromHRP(hrp);
            byte[] enc = new byte[hrpExpanded.Length + data.Length + 6];
            Array.Copy(hrpExpanded, 0, enc, 0, hrpExpanded.Length);
            Array.Copy(data, 0, enc, hrpExpanded.Length, data.Length);
            int mod = Polymod(enc) ^ 1;
            byte[] ret = new byte[6];
            for (int i = 0; i < 6; ++i)
            {
                ret[i] = (byte)((ShiftRight(mod, (5 * (5 - i)))) & 31);
            }
            return ret;
        }

        private static bool VerifyChecksum(string hrp, byte[] data)
        {
            var hrpBytes = GetBytesFromHRP(hrp);
            var hrpLength = hrpBytes.Length;
            var all = new byte[hrpLength + data.Length];
            Array.Copy(hrpBytes, 0, all, 0, hrpLength);
            Array.Copy(data, 0, all, hrpLength, data.Length);
            var pol = Polymod(all);
            return pol == 1;
        }
        #endregion

        ///**
        // * Find the polynomial with value coefficients mod the generator as 30-bit.
        // */
        private static int Polymod(byte[] values)
        {
            int c = 1;
            foreach (byte v_i in values)
            {
                int c0 = (ShiftRight(c, 25)) & 0xff;
                c = ((c & 0x1ffffff) << 5) ^ (v_i & 0xff);
                if ((c0 & 1) != 0) c ^= 0x3b6a57b2;
                if ((c0 & 2) != 0) c ^= 0x26508e6d;
                if ((c0 & 4) != 0) c ^= 0x1ea119fa;
                if ((c0 & 8) != 0) c ^= 0x3d4233dd;
                if ((c0 & 16) != 0) c ^= 0x2a1462b3;
            }
            return c;
        }
        private static byte[] GetBytesFromHRP(string hrp)
        {
            int hrpLength = hrp.Length;
            byte[] ret = new byte[hrpLength * 2 + 1];
            for (int i = 0; i < hrpLength; ++i)
            {
                int c = hrp.ToCharArray()[i] & 0x7f; // Limit to standard 7-bit ASCII
                ret[i] = (byte)((ShiftRight(c, 5)) & 0x07);
                ret[i + hrpLength + 1] = (byte)(c & 0x1f);
            }
            ret[hrpLength] = 0;
            return ret;
        }
        private static int ShiftRight(int value, int pos)
        {
            if (pos != 0)
            {
                // Shift with 0x7FFFFFFF 
                value >>= 1;
                value &= int.MaxValue;
                value >>= pos - 1;
            }

            return value;
        }

        

        #endregion



    }
}
