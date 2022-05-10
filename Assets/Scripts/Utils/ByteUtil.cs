using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MusZil_Core.Utils
{
    public class ByteUtil
    {
        
        #region New Way with .NET libs

        #region Convert int to string

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string LongToBaseN(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Transforms string to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] Transform(string str)
        {
            // Create a new ToBase64Transform object to convert to base 64.
            ToBase64Transform base64Transform = new ToBase64Transform();

            // Create a new byte array with the size of the output block size.
            byte[] outputBytes = new byte[base64Transform.OutputBlockSize];

            // Retrieve the file contents into a byte array.
            byte[] inputBytes = HexStringToByteArray(str);

            // Verify that multiple blocks can not be transformed.
            if (!base64Transform.CanTransformMultipleBlocks)
            {
                // Initializie the offset size.
                int inputOffset = 0;

                // Iterate through inputBytes transforming by blockSize.
                int inputBlockSize = base64Transform.InputBlockSize;

                while (inputBytes.Length - inputOffset > inputBlockSize)
                {
                    base64Transform.TransformBlock(
                        inputBytes,
                        inputOffset,
                        inputBytes.Length - inputOffset,
                        outputBytes,
                        0);

                    inputOffset += base64Transform.InputBlockSize;
                }

                // Transform the final block of data.
                outputBytes = base64Transform.TransformFinalBlock(
                    inputBytes,
                    inputOffset,
                    inputBytes.Length - inputOffset);

                //Determine if the current transform can be reused.
                if (!base64Transform.CanReuseTransform)
                {
                    // Free up any used resources.
                    base64Transform.Clear();
                }
            }
            return outputBytes;
        }

        public static string ConvertHexToValueString(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length * 3);
            foreach (var c in str.ToCharArray())
            {
                LongToBaseN(c, 8);
                sb.Append(c);
            }
            return sb.ToString().ToUpper();

        }


        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
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

            return arr; ;

        }
       

        public static string ConvertByteArrToString(byte[] a, string separator = " ",int pad_L = 8,int pad_R=0)
        {
            return string.Join(separator, a.Select(x => Convert.ToString(x, 2)));
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
        public static List<int> ConvertBits(byte[] data, int fromWidth, int toWidth, bool pad)
        {
            int acc = 0;
            int bits = 0;
            int maxv = (1 << toWidth) - 1;
            List<int> ret = new List<int>();

            for (int i = 0; i < data.Length; i++)
            {
                int value = data[i] & 0xff;
                if (value < 0 || value >> fromWidth != 0)
                {
                    return null;
                }
                acc = (acc << fromWidth) | value;
                bits += fromWidth;
                while (bits >= toWidth)
                {
                    bits -= toWidth;
                    ret.Add((acc >> bits) & maxv);
                }
            }

            if (pad)
            {
                if (bits > 0)
                {
                    ret.Add((acc << (toWidth - bits)) & maxv);
                }
                else if (bits >= fromWidth || ((acc << (toWidth - bits)) & maxv) != 0)
                {
                    return null;
                }
            }

            return ret;

        }

        #endregion
    }
}