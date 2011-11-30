using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LoreSoft.Shared.Security
{
    public static class Encryptor
    {
        #region String
        /// <summary>
        /// Encrypts the string using the Advanced Encryption Standard (AES) symmetric algorithm.
        /// </summary>
        /// <param name="plainText">The plain string to be encypted.</param>
        /// <param name="key">The secret key to use for the symmetric algorithm.</param>
        /// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>A byte array containing the encypted string.</returns>
        public static byte[] EncryptString(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            using (var msEncrypt = new MemoryStream())
            using (var aes = new AesManaged())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                    swEncrypt.Write(plainText);

                return msEncrypt.ToArray();
            }
        }

        public static byte[] EncryptString(string plainText, string keyPhrase)
        {
            // Check arguments.
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(keyPhrase))
                throw new ArgumentNullException("keyPhrase");

            byte[] key = GetKey(keyPhrase);
            byte[] iv = GenerateIV();
            byte[] cipher = EncryptString(plainText, key, iv);

            // join iv and encrypted bytes
            byte[] encrypted = new byte[iv.Length + cipher.Length];

            Buffer.BlockCopy(iv, 0, encrypted, 0, iv.Length);
            Buffer.BlockCopy(cipher, 0, encrypted, iv.Length, cipher.Length);

            return encrypted;
        }

        /// <summary>
        /// Decrypts the string using the Advanced Encryption Standard (AES) symmetric algorithm.
        /// </summary>
        /// <param name="encrypted">The byte array containing the encypted text.</param>
        /// <param name="key">The secret key to use for the symmetric algorithm.</param>
        /// <param name="iv">The initialization vector to use for the symmetric algorithm.</param>
        /// <returns>The decypted string.</returns>
        public static string DecryptString(byte[] encrypted, byte[] key, byte[] iv)
        {
            if (encrypted == null || encrypted.Length <= 0)
                throw new ArgumentNullException("encrypted");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            string plaintext = null;

            using (var aes = new AesManaged())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var msDecrypt = new MemoryStream(encrypted))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                    plaintext = srDecrypt.ReadToEnd();
            }

            return plaintext;
        }

        public static string DecryptString(byte[] encrypted, string keyPhrase)
        {
            if (encrypted == null)
                throw new ArgumentNullException("encrypted");
            if (encrypted.Length <= 16)
                throw new ArgumentOutOfRangeException("encrypted", "The encrypted byte array is to small.");
            if (string.IsNullOrEmpty(keyPhrase))
                throw new ArgumentNullException("keyPhrase");

            byte[] key = GetKey(keyPhrase);
            byte[] iv = new byte[16];
            byte[] cipher = new byte[encrypted.Length - 16];

            Buffer.BlockCopy(encrypted, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(encrypted, iv.Length, cipher, 0, cipher.Length);

            return DecryptString(cipher, key, iv);
        }
        #endregion

        public static byte[] Encrypt(byte[] buffer, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (buffer == null || buffer.Length <= 0)
                throw new ArgumentNullException("buffer");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            using (var msEncrypt = new MemoryStream())
            using (var aes = new AesManaged())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new BinaryWriter(csEncrypt))
                    swEncrypt.Write(buffer, 0, buffer.Length);

                return msEncrypt.ToArray();
            }
        }

        public static byte[] Encrypt(byte[] buffer, string keyPhrase)
        {
            // Check arguments.
            if (buffer == null || buffer.Length <= 0)
                throw new ArgumentNullException("buffer");
            if (string.IsNullOrEmpty(keyPhrase))
                throw new ArgumentNullException("keyPhrase");

            byte[] key = GetKey(keyPhrase);
            byte[] iv = GenerateIV();
            byte[] cipher = Encrypt(buffer, key, iv);

            // join iv and encrypted bytes
            byte[] encrypted = new byte[iv.Length + cipher.Length];

            Buffer.BlockCopy(iv, 0, encrypted, 0, iv.Length);
            Buffer.BlockCopy(cipher, 0, encrypted, iv.Length, cipher.Length);

            return encrypted;
        }

        public static byte[] Decrypt(byte[] encrypted, byte[] key, byte[] iv)
        {
            if (encrypted == null || encrypted.Length <= 0)
                throw new ArgumentNullException("encrypted");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            byte[] buffer = new byte[0x1000];

            using (var writer = new MemoryStream())
            using (var aes = new AesManaged())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var msDecrypt = new MemoryStream(encrypted))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new BinaryReader(csDecrypt))
                {
                    int num;
                    while ((num = srDecrypt.Read(buffer, 0, buffer.Length)) != 0)
                        writer.Write(buffer, 0, num);
                }

                return writer.ToArray();
            }
        }

        public static byte[] Decrypt(byte[] encrypted, string keyPhrase)
        {
            if (encrypted == null)
                throw new ArgumentNullException("encrypted");
            if (encrypted.Length <= 16)
                throw new ArgumentOutOfRangeException("encrypted", "The encrypted byte array is to small.");
            if (string.IsNullOrEmpty(keyPhrase))
                throw new ArgumentNullException("keyPhrase");

            byte[] key = GetKey(keyPhrase);
            byte[] iv = new byte[16];
            byte[] cipher = new byte[encrypted.Length - 16];

            Buffer.BlockCopy(encrypted, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(encrypted, iv.Length, cipher, 0, cipher.Length);

            return Decrypt(cipher, key, iv);
        }

        /// <summary>
        /// Generates a random initialization vector (IV) to use for the symmetric algorithm.
        /// </summary>
        /// <returns>The initialization vector (IV) to use for the symmetric algorithm.</returns>
        public static byte[] GenerateIV()
        {
            byte[] iv;
            using (var aes = new AesManaged())
            {
                aes.GenerateIV();
                iv = aes.IV;
            }
            return iv;
        }

        /// <summary>
        /// Generates a random key to use for the symmetric algorithm. 
        /// </summary>
        /// <returns>The secret key used for the symmetric algorithm.</returns>
        public static byte[] GenerateKey()
        {
            byte[] key;
            using (var aes = new AesManaged())
            {
                aes.GenerateKey();
                key = aes.Key;
            }
            return key;
        }

        public static byte[] GetKey(string keyPhrase)
        {
            byte[] key;

            var ms = new MemoryStream();
            using (var sw = new StreamWriter(ms))
                sw.Write(keyPhrase);

            using (var sha = new SHA256Managed())
                key = sha.ComputeHash(ms.ToArray());

            return key;
        }

    }
}
