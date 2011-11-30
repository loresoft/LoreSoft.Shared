using System;
using System.Runtime.InteropServices;

namespace LoreSoft.Shared.Extensions
{
    public static class NumberExtensions
    {
        #region Between
        public static bool Between(this byte value, byte start, byte end)
        {
            return (start <= value && value <= end);
        }
        public static bool Between(this int value, int start, int end)
        {
            return (start <= value && value <= end);
        }
        public static bool Between(this long value, long start, long end)
        {
            return (start <= value && value <= end);
        }
        public static bool Between(this short value, short start, short end)
        {
            return (start <= value && value <= end);
        }
        public static bool Between(this double value, double start, double end)
        {
            return (start <= value && value <= end);
        }
        public static bool Between(this float value, float start, float end)
        {
            return (start <= value && value <= end);
        }
        public static bool Between(this decimal value, decimal start, decimal end)
        {
            return (start <= value && value <= end);
        }
        #endregion

        #region Fit
        public static byte Fit(this byte value, byte start, byte end)
        {
            return Math.Min(end, Math.Max(start, value));
        }
        public static int Fit(this int value, int start, int end)
        {
            return Math.Min(end, Math.Max(start, value));
        }
        public static long Fit(this long value, long start, long end)
        {
            return Math.Min(end, Math.Max(start, value));
        }
        public static short Fit(this short value, short start, short end)
        {
            return Math.Min(end, Math.Max(start, value));
        }
        public static double Fit(this double value, double start, double end)
        {
            return Math.Min(end, Math.Max(start, value));
        }
        public static float Fit(this float value, float start, float end)
        {
            return Math.Min(end, Math.Max(start, value));
        }
        public static decimal Fit(this decimal value, decimal start, decimal end)
        {
            return Math.Min(end, Math.Max(start, value));
        }
        #endregion

        #region Round
        public static decimal Round(this decimal value)
        {
            return Math.Round(value);
        }

        public static decimal Round(this decimal value, int decimals)
        {
            return Math.Round(value, decimals);
        }

        public static double Round(this double value)
        {
            return Math.Round(value);
        }

        public static double Round(this double value, int decimals)
        {
            return Math.Round(value, decimals);
        }
        #endregion

        #region ToPercent
        public static decimal ToPercent(this decimal value)
        {
            return Math.Round(value * 100);
        }

        public static decimal ToPercent(this decimal value, int decimals)
        {
            return Math.Round(value * 100, decimals);
        }

        public static double ToPercent(this double value)
        {
            return Math.Round(value * 100);
        }

        public static double ToPercent(this double value, int decimals)
        {
            return Math.Round(value * 100, decimals);
        }
        #endregion

        #region FromPercent
        public static decimal FromPercent(this decimal value)
        {
            return FromPercent(value, 2);
        }

        public static decimal FromPercent(this decimal value, int decimals)
        {
            return Math.Round(value / 100, decimals);
        }

        public static double FromPercent(this double value)
        {
            return FromPercent(value, 2);
        }

        public static double FromPercent(this double value, int decimals)
        {
            return Math.Round(value / 100, decimals);
        }
        #endregion

        #region Epsilon
        // Const values come from sdk\inc\crt\float.h
        // smallest such that 1.0+Epsilon != 1.0
        public const double Epsilon = 2.2204460492503131e-016;

        /// <summary>
        /// Check if a number is zero.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>True if the number is zero, false otherwise.</returns>
        public static bool IsZero(this double value)
        {
            // We actually consider anything within an order of magnitude of epsilon to be zero
            return Math.Abs(value) < 10.0 * Epsilon;
        }

        /// <summary>
        /// Check if a number is one.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>True if the number is one, false otherwise.</returns>
        public static bool IsOne(this double value)
        {
            return Math.Abs(value - 1.0) < 10.0 * Epsilon;
        }

        /// <summary>
        /// Check if a number isn't really a number.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>
        /// True if the number is not a number, false if it is a number.
        /// </returns>
        public static bool IsNaN(this double value)
        {
            // Get the double as an unsigned long
            NanUnion union = new NanUnion { FloatingValue = value };

            // An IEEE 754 double precision floating point number is NaN if its
            // exponent equals 2047 and it has a non-zero mantissa.
            ulong exponent = union.IntegerValue & 0xfff0000000000000L;
            if ((exponent != 0x7ff0000000000000L) && (exponent != 0xfff0000000000000L))
            {
                return false;
            }
            ulong mantissa = union.IntegerValue & 0x000fffffffffffffL;
            return mantissa != 0L;
        }

        /// <summary>
        /// Determine if one number is greater than another.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is greater than the second, false otherwise.
        /// </returns>
        public static bool IsGreaterThan(this double left, double right)
        {
            return (left > right) && !AreClose(left, right);
        }

        /// <summary>
        /// Determine if one number is less than another.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is less than the second, false otherwise.
        /// </returns>
        public static bool IsLessThan(this double left, double right)
        {
            return (left < right) && !AreClose(left, right);
        }

        /// <summary>
        /// Determine if one number is greater than or close to another.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is greater than or close to the second, false otherwise.
        /// </returns>
        public static bool IsGreaterThanOrClose(this double left, double right)
        {
            return (left > right) || AreClose(left, right);
        }

        /// <summary>
        /// Determine if one number is less than or close to another.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is less than or close to the second, false otherwise.
        /// </returns>
        public static bool IsLessThanOrClose(this double left, double right)
        {
            return (left < right) || AreClose(left, right);
        }

        /// <summary>
        /// Determines whether the value is between zero and one.
        /// </summary>
        /// <param name="value">The number value.</param>
        /// <returns>
        ///   <c>true</c> if the value is between zero and one; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBetweenZeroAndOne(this double value)
        {
            return (IsGreaterThanOrClose(value, 0) && IsLessThanOrClose(value, 1));
        }

        /// <summary>
        /// Determine if two numbers are close in value.
        /// </summary>
        /// <param name="left">First number.</param>
        /// <param name="right">Second number.</param>
        /// <returns>
        /// True if the first number is close in value to the second, false otherwise.
        /// </returns>
        public static bool AreClose(this double left, double right)
        {
            //In case they are Infinities (then epsilon check does not work)
            if (left == right)
            {
                return true;
            }

            //computes (|left-right| / (|left| + |right| + 10.0)) < Epsilon
            double a = (Math.Abs(left) + Math.Abs(right) + 10.0) * Epsilon;
            double b = left - right;
            return (-a < b) && (a > b);
        }

        /// <summary>
        /// NanUnion is a C++ style type union used for efficiently converting
        /// a double into an unsigned long, whose bits can be easily
        /// manipulated.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            /// <summary>
            /// Floating point representation of the union.
            /// </summary>
            [FieldOffset(0)]
            internal double FloatingValue;

            /// <summary>
            /// Integer representation of the union.
            /// </summary>
            [FieldOffset(0)]
            internal ulong IntegerValue;
        }
        #endregion

    }
}
