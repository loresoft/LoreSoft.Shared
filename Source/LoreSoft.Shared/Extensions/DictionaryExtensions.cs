using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace LoreSoft.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Adds a key/value pair to the Dictionary if the key does not already exist.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key.</param>
        /// <returns>The value for the key. This will be either the existing value for the key if the key is already in the dictionary, or the new value for the key as returned by valueFactory if the key was not in the dictionary.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
                return value;

            value = valueFactory(key);
            dictionary.Add(key, value);

            return value;
        }

        /// <summary>
        /// Serializes the specified dictionary and writes the XML document to the specified Stream.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to serialize.</param>
        /// <param name="stream">The Stream used to write the XML document.</param>
        public static void Serialize<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Stream stream)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (stream == null)
                throw new ArgumentNullException("stream");

            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(stream, settings);
            Serialize(dictionary, writer);
        }

        /// <summary>
        /// Serializes the specified dictionary and writes the XML document to the specified TextWriter.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to serialize.</param>
        /// <param name="textWriter">The TextWriter used to write the XML document.</param>
        public static void Serialize<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TextWriter textWriter)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (textWriter == null)
                throw new ArgumentNullException("textWriter");

            var settings = new XmlWriterSettings { Indent = true };
            var writer = XmlWriter.Create(textWriter, settings);
            Serialize(dictionary, writer);
        }

        /// <summary>
        /// Serializes the specified dictionary and writes the XML document to the specified XmlWriter.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to serialize.</param>
        /// <param name="xmlWriter">The XmlWriter used to write the XML document.</param>
        public static void Serialize<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, XmlWriter xmlWriter)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (xmlWriter == null)
                throw new ArgumentNullException("xmlWriter");

            var knownTypes = new HashSet<Type>();
            foreach (var pair in dictionary)
            {
                Type keyType = pair.Key.GetType();
                if (!keyType.IsPrimitive && keyType != typeof(string))
                    knownTypes.Add(keyType);

                if (Equals(pair.Value, null))
                    continue;

                Type valueType = pair.Value.GetType();
                if (!valueType.IsPrimitive && valueType != typeof(string))
                    knownTypes.Add(valueType);
            }

            xmlWriter.WriteStartElement("dictionary");
            xmlWriter.WriteStartElement("types");
            foreach (var knownType in knownTypes)
                xmlWriter.WriteElementString("type", knownType.AssemblyQualifiedName);
            xmlWriter.WriteEndElement(); // types

            var serializer = new DataContractSerializer(typeof(IDictionary<TKey, TValue>), "graph", string.Empty, knownTypes);
            serializer.WriteObject(xmlWriter, dictionary);

            xmlWriter.WriteEndElement(); //dictionary

            xmlWriter.Flush();
        }


        /// <summary>
        /// Deserializes the XML document contained by the specified Stream.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to deserialize into.</param>
        /// <param name="stream">The Stream used to read the XML document.</param>
        public static void Deserialize<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Stream stream)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (stream == null)
                throw new ArgumentNullException("stream");

            var reader = XmlReader.Create(stream);
            Deserialize(dictionary, reader);
        }

        /// <summary>
        /// Deserializes the XML document contained by the specified TextReader.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to deserialize into.</param>
        /// <param name="textReader">The TextReader used to read the XML document.</param>
        public static void Deserialize<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TextReader textReader)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (textReader == null)
                throw new ArgumentNullException("textReader");

            var reader = XmlReader.Create(textReader);
            Deserialize(dictionary, reader);
        }

        /// <summary>
        /// Deserializes the XML document contained by the specified XmlReader.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary to deserialize into.</param>
        /// <param name="xmlReader">The XmlReader used to read the XML document.</param>
        public static void Deserialize<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, XmlReader xmlReader)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");
            if (xmlReader == null)
                throw new ArgumentNullException("xmlReader");

            var knownTypes = new HashSet<Type>();

            try
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    if (xmlReader.LocalName == "type")
                    {
                        string name = xmlReader.ReadElementContentAsString();
                        Type type = Type.GetType(name, false);
                        if (type != null)
                            knownTypes.Add(type);
                    }
                    else if (xmlReader.LocalName == "graph")
                    {
                        var serializer = new DataContractSerializer(typeof(IDictionary<TKey, TValue>), "graph", string.Empty, knownTypes);
                        var graph = serializer.ReadObject(xmlReader) as IDictionary<TKey, TValue>;
                        if (graph != null)
                            foreach (var pair in graph)
                                dictionary[pair.Key] = pair.Value;
                    }
                }
            }
            catch (XmlException)
            {
                // ignore XmlException
            }
        }
    }
}
