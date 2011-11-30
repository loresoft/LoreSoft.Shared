using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LoreSoft.Shared.Extensions;
using LoreSoft.Shared.Security;

namespace LoreSoft.Shared.Configuration
{
    public class ConfigurationDictionary
        : Dictionary<string, string>
    {
        public ConfigurationDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        { }

        public ConfigurationDictionary(int capacity)
            : base(capacity, StringComparer.OrdinalIgnoreCase)
        { }

        public ConfigurationDictionary(IDictionary<string, string> dictionary)
            : base(dictionary, StringComparer.OrdinalIgnoreCase)
        { }


        public bool TryGetInt32(string key, out int value)
        {
            value = 0;
            string v;

            if (!TryGetValue(key, out v))
                return false;

            return int.TryParse(v, out value);
        }

        public bool TryGetInt64(string key, out long value)
        {
            value = 0;
            string v;

            if (!TryGetValue(key, out v))
                return false;

            return long.TryParse(v, out value);
        }

        public bool TryGetDouble(string key, out double value)
        {
            value = 0;
            string v;

            if (!TryGetValue(key, out v))
                return false;

            return double.TryParse(v, out value);
        }

        public bool TryGetBoolean(string key, out bool value)
        {
            value = false;
            string v;

            if (!TryGetValue(key, out v))
                return false;

            return bool.TryParse(v, out value);
        }

        public bool TryGetDateTime(string key, out DateTime value)
        {
            value = DateTime.MinValue;
            string v;

            if (!TryGetValue(key, out v))
                return false;

            return DateTime.TryParse(v, out value);
        }

        public bool TryGetTimeSpan(string key, out TimeSpan value)
        {
            value = TimeSpan.Zero;
            string v;

            if (!TryGetValue(key, out v))
                return false;

            return TimeSpan.TryParse(v, out value);
        }

        public bool TryGetGuid(string key, out Guid value)
        {
            value = Guid.Empty;
            string v;

            if (!TryGetValue(key, out v))
                return false;

            return Guid.TryParse(v, out value);
        }


        public string Encrypt(string keyPhrase)
        {
            byte[] buffer;

            // serialize
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write(Count);
                foreach (var pair in this)
                {
                    // write key, then value
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }

                buffer = ms.ToArray();
            }

            byte[] encrypted = Encryptor.Encrypt(buffer, keyPhrase);
            return encrypted.ToHex();
        }

        public void Decrypt(string encryptedText, string keyPhrase)
        {
            byte[] encrypted = encryptedText.ToByteArray();
            byte[] buffer = Encryptor.Decrypt(encrypted, keyPhrase);

            using (var ms = new MemoryStream(buffer))
            using (var reader = new BinaryReader(ms))
            {
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    string value = reader.ReadString();

                    this[key] = value;
                }
            }
        }
    }
}
