using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Linq;

namespace LoreSoft.Shared.Extensions
{
    /// <summary>
    /// A class to help with Enum Flags.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Determines whether any flag is on for the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to check if the flag is on.</param>
        /// <param name="flag">The flag to check for in the mask.</param>
        /// <returns>
        /// 	<c>true</c> if any flag is on for the specified mask; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAnyFlagOn<T>(this Enum mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            return (maskInt & flagInt) != 0;
        }

        /// <summary>
        /// Determines whether the flag is on for the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to check if the flag is on.</param>
        /// <param name="flag">The flag to check for in the mask.</param>
        /// <returns>
        /// 	<c>true</c> if the flag is on for the specified mask; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFlagOn<T>(this Enum mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            return (maskInt & flagInt) == flagInt;
        }

        /// <summary>
        /// Sets the flag on in the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to set flag on.</param>
        /// <param name="flag">The flag to set.</param>
        /// <returns>The mask with the flag set to on.</returns>
        public static T SetFlagOn<T>(this Enum mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            maskInt |= flagInt;

            return ConvertFlag<T>(maskInt);
        }

        /// <summary>
        /// Sets the flag off in the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to set flag off.</param>
        /// <param name="flag">The flag to set.</param>
        /// <returns>The mask with the flag set to off.</returns>
        public static T SetFlagOff<T>(this Enum mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            maskInt &= ~flagInt;

            return ConvertFlag<T>(maskInt);
        }

        /// <summary>
        /// Toggles the flag in the specified mask.
        /// </summary>
        /// <typeparam name="T">The flag type.</typeparam>
        /// <param name="mask">The mask to toggle the flag against.</param>
        /// <param name="flag">The flag to toggle.</param>
        /// <returns>The mask with the flag set in the opposite position then it was.</returns>
        public static T ToggleFlag<T>(this Enum mask, T flag)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            ulong flagInt = Convert.ToUInt64(flag);
            ulong maskInt = Convert.ToUInt64(mask);

            maskInt ^= flagInt;

            return ConvertFlag<T>(maskInt);
        }

        /// <summary>
        /// Gets the string hex of the enum.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="enum">The enum to get the string hex from.</param>
        /// <returns></returns>
        public static string ToStringHex<T>(this Enum @enum)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            return string.Format("{0:x8}", @enum); //hex            
        }

        /// <summary>
        /// Tries to get an enum from a string.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        /// <param name="input">The input string.</param>
        /// <param name="returnValue">The return enum value.</param>
        /// <returns>
        /// 	<c>true</c> if the string was able to be parsed to an enum; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryParseEnum<T>(this Enum value, string input, out T returnValue)
             where T : struct, IComparable, IFormattable, IConvertible
        {
            returnValue = default(T);
            if (input.IsNullOrEmpty())
                return false;

            Type t = typeof(T);
            if (t.IsEnum && Enum.IsDefined(t, input))
            {
                returnValue = (T)Enum.Parse(t, input, true);
                return true;
            }
            return false;
        }

        private static T ConvertFlag<T>(ulong maskInt)
        {
            Type t = typeof(T);
            if (t.IsEnum)
                return (T)Enum.ToObject(t, maskInt);

            return (T)Convert.ChangeType(maskInt, t, Thread.CurrentThread.CurrentUICulture);
        }

#if SILVERLIGHT
    public static IEnumerable<string> GetNames(Type enumType)
    {
      if (enumType == null)
        throw new ArgumentNullException("enumType");

      return enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
        .OrderBy(f => f.GetRawConstantValue())
        .Select(f => f.Name);
    }

    public static IEnumerable<object> GetValues(Type enumType)
    {
      if (enumType == null)
        throw new ArgumentNullException("enumType");

      return enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
        .OrderBy(f => f.GetRawConstantValue())
        .Select(f => f.GetRawConstantValue());
    }
#endif

    }
}
