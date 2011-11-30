using System;
using System.Security.Cryptography;

namespace LoreSoft.Shared.Security
{
    public static class PasswordGenerator
    {
        private static readonly char[] _vowels = new[] { 'a', 'e', 'i', 'o', 'u' };
        private static readonly char[] _consonants = new[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'r', 's', 't', 'v' };
        private static readonly char[] _doubleConsonants = new[] { 'c', 'd', 'f', 'g', 'l', 'm', 'n', 'p', 'r', 's', 't' };
        private static readonly Random _random = new Random();

        public static string Generate(int passwordLength)
        {
            bool wroteConsonant = false;
            int counter = 0;
            var password = new System.Text.StringBuilder();

            for (counter = 0; counter <= passwordLength; counter++)
            {
                if (password.Length > 0 & (wroteConsonant == false) & (_random.Next(100) < 10))
                {
                    password.Append(_doubleConsonants[_random.Next(_doubleConsonants.Length)], 2);
                    counter += 1;
                    wroteConsonant = true;
                }
                else
                {
                    if ((wroteConsonant == false) & (_random.Next(100) < 90))
                    {
                        password.Append(_consonants[_random.Next(_consonants.Length)]);
                        wroteConsonant = true;
                    }
                    else
                    {
                        password.Append(_vowels[_random.Next(_vowels.Length)]);
                        wroteConsonant = false;
                    }
                }
            }

            password.Length = passwordLength;
            return password.ToString();
        }
    }
}
