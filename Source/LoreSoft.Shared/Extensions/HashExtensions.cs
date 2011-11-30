using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LoreSoft.Shared.Extensions
{
    /// <summary>
    /// Hash Extension methods
    /// </summary>
    public static class HashExtensions
    {
#if !SILVERLIGHT
        /// <summary>Compute MD5 hash on input string</summary>
        /// <param name="input">The string to compute hash on.</param>
        /// <returns>The hash as a 32 character hexadecimal string.</returns>
        public static string ToMD5(this string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            MD5 hasher = MD5.Create();
            byte[] data = hasher.ComputeHash(Encoding.Default.GetBytes(input));
            return ToHex(data);
        }

        /// <summary>Compute MD5 hash on an array of bytes.</summary>
        /// <param name="buffer">The buffer of bytes.</param>
        /// <returns>The hash as a 32 character hexadecimal string.</returns>
        public static string ToMD5(this byte[] buffer)
        {
            MD5 md5 = MD5.Create();
            return ToHex(md5.ComputeHash(buffer));
        }

        /// <summary>Compute MD5 hash on a stream.</summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The hash as a 32 character hexadecimal string.</returns>
        public static string ToMD5(this Stream stream)
        {
            MD5 md5 = MD5.Create();

            if (stream.Position != 0)
                stream.Position = 0;

            return ToHex(md5.ComputeHash(stream));
        }

        /// <summary>Compute MD5 hash for a file.</summary>
        /// <param name="file">The file to get hash from.</param>
        /// <returns>The hash as a 32 character hexadecimal string.</returns>
        public static string ToMD5(this FileInfo file)
        {
            using (FileStream fs = file.OpenRead())
            {
                return ToMD5(fs);
            }
        }
#endif


        /// <summary>Compute SHA1 hash on input string</summary>
        /// <param name="input">The string to compute hash on.</param>
        /// <returns>The hash as a hexadecimal string.</returns>
        public static string ToSHA1(this string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            var hasher = new SHA1Managed();
            byte[] data = hasher.ComputeHash(Encoding.Unicode.GetBytes(input));

            return ToHex(data);
        }

        /// <summary>Compute SHA1 hash on input string</summary>
        /// <param name="input">The string to compute hash on.</param>
        /// <returns>The hash as a hexadecimal string.</returns>
        public static string ToSHA1(this Stream input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            var hasher = new SHA1Managed();
            byte[] data = hasher.ComputeHash(input);

            return ToHex(data);
        }

        /// <summary>
        /// Compute SHA1 hash on input string
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The hash as a hexadecimal string.</returns>
        public static string ToSHA1(this byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            var hasher = new SHA1Managed();
            byte[] data = hasher.ComputeHash(buffer);

            return ToHex(data);
        }

        /// <summary>Compute SHA1 hash on input string</summary>
        /// <param name="file">The file to get hash from.</param>
        /// <returns>The hash as a hexadecimal string.</returns>
        public static string ToSHA1(this FileInfo file)
        {
            using (FileStream fs = file.OpenRead())
            {
                return ToSHA1(fs);
            }

        }

        /// <summary>Compute SHA1 hash on input string</summary>
        /// <param name="buffer">The string to compute hash on.</param>
        /// <returns>The hash as a hexadecimal string.</returns>
        public static string ToSHA1(this StringBuilder buffer)
        {
            return ToSHA1(buffer.ToString());
        }

        /// <summary>
        /// Converts a byte array to Hexadecimal.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <returns>Hexadecimal string of the byte array.</returns>
        public static string ToHex(this byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)       // <-- use for loop is faster than foreach   
                hex.Append(bytes[i].ToString("X2"));       // <-- ToString is faster than AppendFormat   

            return hex.ToString();
        }

        /// <summary>
        /// Converts a byte array to Base64.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <returns>Base64 string of the byte array.</returns>
        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts a hexadecimal string into a byte array.
        /// </summary>
        /// <param name="hex">The hex string.</param>
        /// <returns>A byte array.</returns>
        public static byte[] ToByteArray(this string hex)
        {
            if ((hex.Length % 2) != 0)
                throw new FormatException("The hex string length must be in multiple of 2");

            return Enumerable.Range(0, hex.Length).
                   Where(x => 0 == x % 2).
                   Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).
                   ToArray();
        }
    }
}
