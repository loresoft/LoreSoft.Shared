using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace LoreSoft.Shared.Collections
{
    /// <summary>
    /// Represents a collection of keys and values where the key can have multiple values.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class MultiDictionary<TKey, TValue>
      : Dictionary<TKey, HashSet<TValue>>, ILookup<TKey, TValue>
    {
        private readonly IEqualityComparer<TValue> _valueComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LoreSoft.Shared.Collections.MultiDictionary`2"/> class.
        /// </summary>
        public MultiDictionary()
            : this(0, null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="MultiDictionary&lt;TKey, TValue&gt;"/> can contain.</param>
        public MultiDictionary(int capacity)
            : this(capacity, null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="keyComparer">
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing keys, or <see langword="null"/>
        /// to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/> for the type of the key.
        /// </param>
        /// <param name="valueComparer">
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing values, or <see langword="null"/>
        /// to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/> for the type of the value.
        /// </param>
        public MultiDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            : this(0, keyComparer, valueComparer)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="MultiDictionary&lt;TKey, TValue&gt;"/> can contain.</param>
        /// <param name="keyComparer">
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing keys, or <see langword="null"/>
        /// to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/> for the type of the key.
        /// </param>
        public MultiDictionary(int capacity, IEqualityComparer<TKey> keyComparer)
            : this(capacity, keyComparer, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="MultiDictionary&lt;TKey, TValue&gt;"/> can contain.</param>
        /// <param name="keyComparer">
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing keys, or <see langword="null"/>
        /// to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/> for the type of the key.
        /// </param>
        /// <param name="valueComparer">
        /// The <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing values, or <see langword="null"/>
        /// to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/> for the type of the value.
        /// </param>
        public MultiDictionary(int capacity, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
            : base(capacity, keyComparer)
        {
            _valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDictionary&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="info">
        /// A <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object containing the information required to serialize the <see cref="T:LoreSoft.Shared.Collections.MultiDictionary`2" />.
        /// </param>
        /// <param name="context">
        /// A <see cref="T:System.Runtime.Serialization.StreamingContext" /> structure containing the source and destination of the serialized stream associated with the <see cref="T:LoreSoft.Shared.Collections.MultiDictionary`2" />.
        /// </param>
        protected MultiDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
#endif

        /// <summary>
        /// Gets the <see cref="T:System.Collections.Generic.IEqualityComparer`1"/> implementation to use when comparing values, or <see langword="null"/>
        /// to use the default <see cref="T:System.Collections.Generic.EqualityComparer`1"/> for the type of the value.
        /// </summary>
        public IEqualityComparer<TValue> ValueComparer
        {
            get { return _valueComparer; }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">
        /// The key of the element to add.
        /// </param>
        /// <param name="value">
        /// The value of the element to add.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            HashSet<TValue> container;
            if (!TryGetValue(key, out container))
            {
                container = new HashSet<TValue>(ValueComparer);
                Add(key, container);
            }
            container.Add(value);
        }

        /// <summary>
        /// Adds the specified key and values to the dictionary.
        /// </summary>
        /// <param name="key">
        /// The key of the element to add.
        /// </param>
        /// <param name="values">
        /// The values of the element to add.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.
        /// </exception>
        public void AddRange(TKey key, IEnumerable<TValue> values)
        {
            if (null == values)
                return;

            foreach (TValue value in values)
                Add(key, value);
        }

        /// <summary>
        /// Adds the specified <see cref="T:System.Collections.Generic.IDictionary`2"/> to this dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to add.</param>
        public void AddRange(IDictionary<TKey, IEnumerable<TValue>> dictionary)
        {
            if (null == dictionary)
                return;

            foreach (var pair in dictionary)
                foreach (TValue value in pair.Value)
                    Add(pair.Key, value);
        }

        /// <summary>
        /// Determines whether the <see cref="T:LoreSoft.Shared.Collections.MultiDictionary`2" /> contains a specific key and value.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="T:LoreSoft.Shared.Collections.MultiDictionary`2" /> contains an element with the specified value; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="key">
        /// The key to locate in the <see cref="T:LoreSoft.Shared.Collections.MultiDictionary`2" />.
        /// </param>
        /// <param name="value">
        /// The value to locate in the <see cref="T:LoreSoft.Shared.Collections.MultiDictionary`2" />.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is <c>null</c>.
        /// </exception>
        public bool ContainsValue(TKey key, TValue value)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            bool contains = false;
            HashSet<TValue> values;
            if (TryGetValue(key, out values))
                contains = values.Contains(value);

            return contains;
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="T:LoreSoft.Shared.Collections.MultiDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.
        /// This method returns <c>false</c> if <paramref name="key"/> or <paramref name="value"/> is not found.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
        public bool Remove(TKey key, TValue value)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            HashSet<TValue> container;
            if (!TryGetValue(key, out container))
                return false;

            var b = container.Remove(value);
            if (container.Count <= 0)
                Remove(key);

            return b;
        }

        /// <summary>
        /// Gets the values associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key, if the key is found; otherwise, the default value for the type of the value.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>
        public HashSet<TValue> GetValues(TKey key)
        {
            if (null == key)
                throw new ArgumentNullException("key");

            HashSet<TValue> container;
            TryGetValue(key, out container);
            return container;
        }

        #region ILookup<TKey,TValue> Members
        /// <summary>
        /// Determines whether a specified key exists in the <see cref="T:System.Linq.ILookup`2"/>.
        /// </summary>
        /// <param name="key">The key to search for in the <see cref="T:System.Linq.ILookup`2"/>.</param>
        /// <returns>
        /// <c>true</c> if key is in the <see cref="T:System.Linq.ILookup`2"/>; otherwise, <c>false</c>.
        /// </returns>
        bool ILookup<TKey, TValue>.Contains(TKey key)
        {
            return ContainsKey(key);
        }

        /// <summary>
        /// Gets the number of key/value collection pairs in the <see cref="T:System.Linq.ILookup`2"/>.
        /// </summary>
        int ILookup<TKey, TValue>.Count
        {
            get { return Count; }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// The value associated with the specified key.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is <c>null</c>.</exception>    
        IEnumerable<TValue> ILookup<TKey, TValue>.this[TKey key]
        {
            get
            {
                HashSet<TValue> value;
                TryGetValue(key, out value);
                return value ?? Enumerable.Empty<TValue>();
            }
        }
        #endregion

        #region IEnumerable<IGrouping<TKey,TValue>> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<IGrouping<TKey, TValue>> IEnumerable<IGrouping<TKey, TValue>>.GetEnumerator()
        {
            foreach (KeyValuePair<TKey, HashSet<TValue>> pair in this)
                yield return new Grouping<TKey, TValue>(pair.Key, pair.Value);
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }

    /// <summary>
    /// Represents an item in a group of objects that have a common key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public class Grouping<TKey, TElement>
      : IGrouping<TKey, TElement>
    {
        private readonly TKey _key;
        private readonly IEnumerable<TElement> _elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grouping&lt;TKey, TElement&gt;"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="elements">The elements.</param>
        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            _key = key;
            _elements = elements;
        }

        /// <summary>
        /// Gets the key of the <see cref="T:System.Linq.IGrouping`2"/>.
        /// </summary>
        TKey IGrouping<TKey, TElement>.Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}
