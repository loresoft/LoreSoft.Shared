using System;
using System.Collections.Generic;
using System.Numerics;

namespace LoreSoft.Shared.Text
{
    /// <summary>
    /// A generic base encoding converter.
    /// </summary>
    public static class BaseConvert
    {
        /// <summary>Default base digits for base 36 encoding.</summary>
        public const string Base36 = "0123456789abcdefghijklmnopqrstuvwxyz";
        /// <summary>Default base digits for base 62 encoding.</summary>
        public const string Base62 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to its equivalent string representation that is encoded with base digits specified.
        /// </summary>
        /// <param name="value">A <see cref="BigInteger"/> that will be encoded.</param>
        /// <param name="baseDigits">The base digits used to encode with.</param>
        /// <returns>The string representation, in base digits, of the contents of <paramref name="value"/>.</returns>
        public static string ToBaseString(BigInteger value, string baseDigits)
        {
            if (baseDigits == null)
                throw new ArgumentNullException("baseDigits");
            if (baseDigits.Length < 2)
                throw new ArgumentOutOfRangeException("baseDigits", "Base alphabet must have at least two characters.");

            // same functionality as Convert.ToBase64String
            if (value == BigInteger.Zero)
                return string.Empty;

            bool isNegative = value.Sign == -1;
            value = isNegative ? -value : value;

            int length = baseDigits.Length;
            var result = new Stack<char>();

            do
            {
                BigInteger remainder;
                value = BigInteger.DivRem(value, length, out remainder);
                result.Push(baseDigits[(int)remainder]);
            } while (value > 0);


            // if the number is negative, add the sign.
            if (isNegative)
                result.Push('-');

            // reverse it
            return new string(result.ToArray());
        }

        /// <summary>
        /// Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with base digits specified.
        /// </summary>
        /// <param name="inArray">An array of 8-bit unsigned integers.</param>
        /// <param name="baseDigits">The base digits used to encode with.</param>
        /// <returns>The string representation, in base digits, of the contents of <paramref name="inArray"/>.</returns>
        public static string ToBaseString(byte[] inArray, string baseDigits)
        {
            if (inArray == null)
                throw new ArgumentNullException("inArray");
            if (baseDigits == null)
                throw new ArgumentNullException("baseDigits");
            if (baseDigits.Length < 2)
                throw new ArgumentOutOfRangeException("baseDigits", "Base alphabet must have at least two characters.");

            // same functionality as Convert.ToBase64String
            if (inArray.Length == 0)
                return string.Empty;

            var value = new BigInteger(inArray);
            return ToBaseString(value, baseDigits);
        }

        /// <summary>
        /// Converts the specified string, which encodes binary data as base digits, to an equivalent <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="baseDigits">The base digits used to encode with.</param>
        /// <returns>A <see cref="BigInteger"/> that is equivalent to <paramref name="value"/>.</returns>
        public static BigInteger FromBaseString(string value, string baseDigits)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (baseDigits == null)
                throw new ArgumentNullException("baseDigits");
            if (baseDigits.Length < 2)
                throw new ArgumentOutOfRangeException("baseDigits", "Base alphabet must have at least two characters.");

            if (string.IsNullOrWhiteSpace(value))
                return BigInteger.Zero;

            int index = 0;
            // skip leading white space
            while (index < value.Length && Char.IsWhiteSpace(value[index]))
                ++index;

            if (index >= value.Length)
                throw new FormatException("Input string was not in the correct format.");

            bool isNegative = value[index] == '-';

            // get the sign if it's there.
            if (value[index] == '+' || value[index] == '-')
                ++index;

            // Make sure there's at least one digit
            if (index >= value.Length)
                throw new FormatException("Input string was not in the correct format.");

            BigInteger result = BigInteger.Zero;

            // Parse the digits.
            while (index < value.Length)
            {
                int n = baseDigits.IndexOf(value[index]);
                if (n < 0)
                    throw new FormatException("Input string was not in the correct format.");

                BigInteger oldResult = result;
                result = unchecked((result * baseDigits.Length) + n);
                if (result < oldResult)
                    throw new OverflowException();

                ++index;
            }

            // skip trailing white space
            while (index < value.Length && Char.IsWhiteSpace(value[index]))
                ++index;

            // and make sure there's nothing else.
            if (index < value.Length)
                throw new FormatException("Input string was not in the correct format.");

            if (isNegative)
                result = -result;

            return result;
        }

        /// <summary>
        /// Converts the specified string, which encodes binary data as base digits, to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="baseDigits">The base digits used to encode with.</param>
        /// <returns>An array of 8-bit unsigned integers that is equivalent to <paramref name="value"/>.</returns>
        public static byte[] FromBaseStringArray(string value, string baseDigits)
        {
            return FromBaseString(value, baseDigits).ToByteArray();
        }
    }
}
