using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LoreSoft.Shared.Text;

namespace LoreSoft.Shared.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex _splitNameRegex = new Regex(@"[\W_]+");
        private static readonly Regex _properWordRegex = new Regex(@"([A-Z][a-z]*)|([0-9]+)");
        private static readonly Regex _whitespace = new Regex(@"\s");

        /// <summary>
        /// Truncates the specified text.
        /// </summary>
        /// <param name="text">The text to truncate.</param>
        /// <param name="keep">The number of characters to keep.</param>
        /// <returns>A truncate string.</returns>
        public static string Truncate(this string text, int keep)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string buffer = NormalizeLineEndings(text);
            if (buffer.Length <= keep)
                return buffer;

            return string.Concat(buffer.Substring(0, keep - 3), "...");
        }

        public static string NormalizeLineEndings(this string text)
        {
            return text
                .Replace("\r\n", "\n")
                .Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// Indicates whether the specified String object is null or an empty string
        /// </summary>
        /// <param name="item">A String reference</param>
        /// <returns>
        ///     <c>true</c> if is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string item)
        {
            return string.IsNullOrEmpty(item);
        }

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters
        /// </summary>
        /// <param name="item">A String reference</param>
        /// <returns>
        ///      <c>true</c> if is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrWhiteSpace(this string item)
        {
            if (item == null)
                return true;

            for (int i = 0; i < item.Length; i++)
                if (!char.IsWhiteSpace(item[i]))
                    return false;

            return true;
        }

        /// <summary>
        /// Uses the string as a format
        /// </summary>
        /// <param name="format">A String reference</param>
        /// <param name="args">Object parameters that should be formatted</param>
        /// <returns>Formatted string</returns>
        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(format, args);
        }

        /// <summary>
        /// Applies a format to the item
        /// </summary>
        /// <param name="item">Item to format</param>
        /// <param name="format">Format string</param>
        /// <returns>Formatted string</returns>
        public static string FormatAs(this object item, string format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(format, item);
        }

        /// <summary>
        /// Uses the string as a format.
        /// </summary>
        /// <param name="format">A String reference</param>
        /// <param name="source">Object that should be formatted</param>
        /// <returns>Formatted string</returns>
        public static string FormatName(this string format, object source)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return NameFormatter.Format(format, source);
        }

        /// <summary>
        /// Applies a format to the item
        /// </summary>
        /// <param name="item">Item to format</param>
        /// <param name="format">Format string</param>
        /// <returns>Formatted string</returns>
        public static string FormatNameAs(this object item, string format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return NameFormatter.Format(format, item);
        }

        /// <summary>
        /// Converts a string to use camelCase.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The to camel case. </returns>
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            string output = ToPascalCase(value);
            if (output.Length > 2)
                return char.ToLower(output[0]) + output.Substring(1);

            return output.ToLower();
        }

        /// <summary>
        /// Converts a string to use PascalCase.
        /// </summary>
        /// <param name="value">Text to convert</param>
        /// <returns>The string</returns>
        public static string ToPascalCase(this string value)
        {
            return value.ToPascalCase(_splitNameRegex);
        }

        /// <summary>
        /// Converts a string to use PascalCase.
        /// </summary>
        /// <param name="value">Text to convert</param>
        /// <param name="splitRegex">Regular Expression to split words on.</param>
        /// <returns>The string</returns>
        public static string ToPascalCase(this string value, Regex splitRegex)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var mixedCase = value.IsMixedCase();
            var names = splitRegex.Split(value);
            var output = new StringBuilder();

            if (names.Length > 1)
            {
                foreach (string name in names)
                {
                    if (name.Length > 1)
                    {
                        output.Append(char.ToUpper(name[0]));
                        if (mixedCase)
                            output.Append(name.Substring(1));
                        else
                            output.Append(name.Substring(1).ToLower());
                    }
                    else
                    {
                        output.Append(name);
                    }
                }
            }
            else if (value.Length > 1)
            {
                output.Append(char.ToUpper(value[0]));
                if (mixedCase)
                    output.Append(value.Substring(1));
                else
                    output.Append(value.Substring(1).ToLower());
            }
            else
            {
                output.Append(value.ToUpper());
            }

            return output.ToString();
        }

        /// <summary>
        /// Takes a NameIdentifier and spaces it out into words "Name Identifier".
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The string</returns>
        public static string ToSpacedWords(this string value)
        {
            value = ToPascalCase(value);

            MatchCollection words = _properWordRegex.Matches(value);
            var spacedName = new StringBuilder();
            foreach (Match word in words)
            {
                spacedName.Append(word.Value);
                spacedName.Append(' ');
            }

            // remove last space
            spacedName.Length = spacedName.Length - 1;
            return spacedName.ToString();
        }

        /// <summary>
        /// Removes all whitespace from a string.
        /// </summary>
        /// <param name="s">Initial string.</param>
        /// <returns>String with no whitespace.</returns>
        public static string RemoveWhiteSpace(this string s)
        {
            return _whitespace.Replace(s, String.Empty);
        }

        /// <summary>
        /// Strips NewLines and Tabs
        /// </summary>
        /// <param name="s">The string to strip.</param>
        /// <returns>Stripped string.</returns>
        public static string RemoveInvisible(this string s)
        {
            return s
                .Replace("\r\n", " ")
                .Replace('\n', ' ')
                .Replace('\t', ' ');
        }

        public static string ReplaceFirst(this string s, string find, string replace)
        {
            var i = s.IndexOf(find);
            if (i >= 0)
            {
                var pre = s.Substring(0, i);
                var post = s.Substring(i + find.Length);
                return String.Concat(pre, replace, post);
            }

            return s;
        }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the end of the StringBuilder object.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to append to.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void AppendLine(this StringBuilder sb, string format, params string[] args)
        {
            sb.AppendFormat(format, args);
            sb.AppendLine();
        }

        /// <summary>
        /// Appends a copy of the specified string if <paramref name="condition"/> is met.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to append to.</param>
        /// <param name="text">The string to append.</param>
        /// <param name="condition">The condition delegate to evaluate. If condition is null, String.IsNullOrWhiteSpace method will be used.</param>
        public static void AppendIf(this StringBuilder sb, string text, Func<string, bool> condition = null)
        {
            var c = condition ?? (s => !string.IsNullOrWhiteSpace(s));

            if (c(text))
                sb.Append(text);
        }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator if <paramref name="condition"/> is met.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to append to.</param>
        /// <param name="text">The string to append.</param>
        /// <param name="condition">The condition delegate to evaluate. If condition is null, String.IsNullOrWhiteSpace method will be used.</param>
        public static void AppendLineIf(this StringBuilder sb, string text, Func<string, bool> condition = null)
        {
            var c = condition ?? (s => !string.IsNullOrWhiteSpace(s));

            if (c(text))
                sb.AppendLine(text);
        }

        /// <summary>
        /// Returns true if s contains substring value.
        /// </summary>
        /// <param name="s">Initial value</param>
        /// <param name="value">Substring value</param>
        /// <returns>Boolean</returns>
        public static bool Contains(this string s, string value)
        {
            return s.IndexOf(value) > -1;
        }

        /// <summary>
        /// Returns true if s contains substring value.
        /// </summary>
        /// <param name="s">Initial value</param>
        /// <param name="value">Substring value</param>
        /// <param name="comparison">StringComparison options.</param>
        /// <returns>Boolean</returns>
        public static bool Contains(this string s, string value, StringComparison comparison)
        {
            return s.IndexOf(value, comparison) > -1;
        }

        /// <summary>
        /// Indicates whether a string contains x occurrences of a string. 
        /// </summary>
        /// <param name="s">The string to search.</param>
        /// <param name="value">The string to search for.</param>
        /// <returns>
        ///     <c>true</c> if the string contains at least two occurrences of {value}; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsMultiple(this string s, string value)
        {
            return s.ContainsMultiple(value, 2);
        }

        /// <summary>
        /// Indicates whether a string contains x occurrences of a string. 
        /// </summary>
        /// <param name="s">The string to search.</param>
        /// <param name="value">The string to search for.</param>
        /// <param name="count">The number of occurrences to search for.</param>
        /// <returns>
        ///     <c>true</c> if the string contains at least {count} occurrences of {value}; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsMultiple(this string s, string value, int count)
        {
            if (count == 0)
                return true;

            int index = s.IndexOf(value);
            if (index > -1)
            {
                return s.Substring(index + 1).ContainsMultiple(value, --count);
            }

            return false;
        }

        public static string[] SplitAndTrim(this string s, params string[] separator)
        {
            if (s.IsNullOrEmpty())
                return new string[0];

            var result = ((separator == null) || (separator.Length == 0))
                ? s.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
                : s.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < result.Length; i++)
                result[i] = result[i].Trim();

            return result;
        }

        public static string[] SplitAndTrim(this string s, params char[] separator)
        {
            if (s.IsNullOrEmpty())
                return new string[0];

            var result = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < result.Length; i++)
                result[i] = result[i].Trim();

            return result;

        }

        /// <summary>
        /// Do any of the strings contain both uppercase and lowercase characters?
        /// </summary>
        /// <param name="values">String values.</param>
        /// <returns>True if any contain mixed cases.</returns>
        public static bool IsMixedCase(this IEnumerable<string> values)
        {
            foreach (var value in values)
                if (value.IsMixedCase())
                    return true;

            return false;
        }

        /// <summary>
        /// Does string contain both uppercase and lowercase characters?
        /// </summary>
        /// <param name="s">The value.</param>
        /// <returns>True if contain mixed case.</returns>
        public static bool IsMixedCase(this string s)
        {
            if (s.IsNullOrEmpty())
                return false;

            var containsUpper = false;
            var containsLower = false;

            foreach (var c in s)
            {
                if (Char.IsUpper(c))
                    containsUpper = true;

                if (Char.IsLower(c))
                    containsLower = true;
            }

            return containsLower && containsUpper;
        }
    }
}
