using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LoreSoft.Shared.ComponentModel;

#if SILVERLIGHT
using LoreSoft.Shared.Collections;
#else
using System.Collections.Concurrent;
#endif

namespace LoreSoft.Shared.Caching
{
    /// <summary>
    /// A simple in memory cache supporting item expiration.
    /// </summary>
    public class CacheManager
      : DisposableBase, IEnumerable<KeyValuePair<string, object>>
    {
        private readonly ConcurrentDictionary<string, CacheItem> _cacheStore = new ConcurrentDictionary<string, CacheItem>();

        /// <summary>
        /// A value that indicates that a cache entry has no absolute expiration. 
        /// </summary>
        public static readonly DateTimeOffset InfiniteAbsoluteExpiration = DateTimeOffset.MaxValue;
        /// <summary>
        /// A value that indicates that a cache entry has no sliding expiration time.
        /// </summary>
        public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;

        /// <summary>
        /// Inserts a cache entry into the cache without overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <returns><c>true</c> if insertion succeeded, or <c>false</c> if there is an already an entry in the cache that has the same key as key.</returns>
        public bool Add(string key, object value)
        {
            var cachePolicy = new CachePolicy();
            return AddCacheItem(key, value, cachePolicy);
        }

        /// <summary>
        /// Inserts a cache entry into the cache without overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <returns>
        ///   <c>true</c> if insertion succeeded, or <c>false</c> if there is an already an entry in the cache that has the same key as key.
        /// </returns>
        public bool Add(string key, object value, DateTimeOffset absoluteExpiration)
        {
            var cachePolicy = new CachePolicy { AbsoluteExpiration = absoluteExpiration };
            return AddCacheItem(key, value, cachePolicy);
        }

        /// <summary>
        /// Inserts a cache entry into the cache without overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="slidingExpiration">A span of time within which a cache entry must be accessed before the cache entry is evicted from the cache.</param>
        /// <returns>
        ///   <c>true</c> if insertion succeeded, or <c>false</c> if there is an already an entry in the cache that has the same key as key.
        /// </returns>
        public bool Add(string key, object value, TimeSpan slidingExpiration)
        {
            var cachePolicy = new CachePolicy { SlidingExpiration = slidingExpiration };
            return AddCacheItem(key, value, cachePolicy);
        }

        /// <summary>
        /// Inserts a cache entry into the cache without overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="cachePolicy">An object that contains eviction details for the cache entry.</param>
        /// <returns>
        ///   <c>true</c> if insertion succeeded, or <c>false</c> if there is an already an entry in the cache that has the same key as key.
        /// </returns>
        public bool Add(string key, object value, CachePolicy cachePolicy)
        {
            return AddCacheItem(key, value, cachePolicy);
        }

        /// <summary>
        /// Removes all keys and values from the cache.
        /// </summary>
        public void Clear()
        {
            _cacheStore.Clear();
        }

        /// <summary>
        /// Determines whether a cache entry exists in the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <returns>
        ///   <c>true</c> if the cache contains a cache entry whose key matches key; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string key)
        {
            return _cacheStore.ContainsKey(key);
        }

        /// <summary>
        /// Gets the total number of cache entries in the cache.
        /// </summary>
        public int Count
        {
            get { return _cacheStore.Count; }
        }

        /// <summary>
        /// Gets the cache value for the specified key
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <returns>The cache value for the specified key, if the entry exists; otherwise, <see langword="null"/>.</returns>
        public object Get(string key)
        {
            var item = GetCacheItem(key);
            return item == null ? null : item.Value;
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value if the key was not in the dictionary.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, 
        /// or the new value if the key was not in the dictionary.
        /// </returns>
        public object GetOrAdd(string key, object value)
        {
            var policy = new CachePolicy();
            return GetOrAdd(key, value, policy);
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value if the key was not in the dictionary.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, 
        /// or the new value if the key was not in the dictionary.
        /// </returns>
        public object GetOrAdd(string key, object value, DateTimeOffset absoluteExpiration)
        {
            var policy = new CachePolicy { AbsoluteExpiration = absoluteExpiration };
            return GetOrAdd(key, value, policy);
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value if the key was not in the dictionary.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="slidingExpiration">A span of time within which a cache entry must be accessed before the cache entry is evicted from the cache.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, 
        /// or the new value if the key was not in the dictionary.
        /// </returns>
        public object GetOrAdd(string key, object value, TimeSpan slidingExpiration)
        {
            var policy = new CachePolicy { SlidingExpiration = slidingExpiration };
            return GetOrAdd(key, value, policy);
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value if the key was not in the dictionary.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="cachePolicy">An object that contains eviction details for the cache entry.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, 
        /// or the new value if the key was not in the dictionary.
        /// </returns>
        public object GetOrAdd(string key, object value, CachePolicy cachePolicy)
        {
            return GetOrAddCacheItem(key, k => new CacheItem(k, value, cachePolicy));
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value for the key as returned by <paramref name="valueFactory"/>.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="valueFactory">The function used to generate a value to insert into cache.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, 
        /// or the new value for the key as returned by <paramref name="valueFactory"/> if the key was not in the dictionary.
        /// </returns>
        public object GetOrAdd(string key, Func<string, object> valueFactory)
        {
            return GetOrAdd(key, valueFactory, new CachePolicy());
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value for the key as returned by <paramref name="valueFactory"/>.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="valueFactory">The function used to generate a value to insert into cache.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, 
        /// or the new value for the key as returned by <paramref name="valueFactory"/> if the key was not in the dictionary.
        /// </returns>
        public object GetOrAdd(string key, Func<string, object> valueFactory, DateTimeOffset absoluteExpiration)
        {
            var policy = new CachePolicy { AbsoluteExpiration = absoluteExpiration };
            return GetOrAdd(key, valueFactory, policy);
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value for the key as returned by <paramref name="valueFactory"/>.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="valueFactory">The function used to generate a value to insert into cache.</param>
        /// <param name="slidingExpiration">A span of time within which a cache entry must be accessed before the cache entry is evicted from the cache.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, 
        /// or the new value for the key as returned by <paramref name="valueFactory"/> if the key was not in the dictionary.
        /// </returns>
        public object GetOrAdd(string key, Func<string, object> valueFactory, TimeSpan slidingExpiration)
        {
            var policy = new CachePolicy { SlidingExpiration = slidingExpiration };
            return GetOrAdd(key, valueFactory, policy);
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value for the key as returned by <paramref name="valueFactory"/>.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="valueFactory">The function used to generate a value to insert into cache.</param>
        /// <param name="cachePolicy">An object that contains eviction details for the cache entry.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key is already in the dictionary, 
        /// or the new value for the key as returned by <paramref name="valueFactory"/> if the key was not in the dictionary.
        /// </returns>
        public object GetOrAdd(string key, Func<string, object> valueFactory, CachePolicy cachePolicy)
        {
            var item = GetOrAddCacheItem(key, k => new CacheItem(k, valueFactory(k), cachePolicy));
            return item == null ? null : item.Value;
        }

        /// <summary>
        /// Removes a cache entry from the cache. 
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <returns>If the entry is found in the cache, the removed cache entry; otherwise, <see langword="null"/>.</returns>
        public object Remove(string key)
        {
            var item = RemoveCacheItem(key);
            return item == null ? null : item.Value;
        }

        /// <summary>
        /// Inserts a cache entry into the cache overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        public void Set(string key, object value)
        {
            var policy = new CachePolicy();
            AddOrUpdateCacheItem(key, value, policy);
        }

        /// <summary>
        /// Inserts a cache entry into the cache overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        public void Set(string key, object value, DateTimeOffset absoluteExpiration)
        {
            var policy = new CachePolicy { AbsoluteExpiration = absoluteExpiration };
            AddOrUpdateCacheItem(key, value, policy);
        }

        /// <summary>
        /// Inserts a cache entry into the cache overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="slidingExpiration">A span of time within which a cache entry must be accessed before the cache entry is evicted from the cache.</param>
        public void Set(string key, object value, TimeSpan slidingExpiration)
        {
            var policy = new CachePolicy { SlidingExpiration = slidingExpiration };
            AddOrUpdateCacheItem(key, value, policy);
        }

        /// <summary>
        /// Inserts a cache entry into the cache overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="cachePolicy">An object that contains eviction details for the cache entry.</param>
        public void Set(string key, object value, CachePolicy cachePolicy)
        {
            AddOrUpdateCacheItem(key, value, cachePolicy);
        }

        /// <summary>
        /// Gets or sets a cache entry with the specified key.
        /// </summary>
        public object this[string key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        #region Cache Store Methods
        /// <summary>
        /// Inserts a cache entry into the cache without overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="cachePolicy">An object that contains eviction details for the cache entry.</param>
        /// <returns>
        ///   <c>true</c> if insertion succeeded, or <c>false</c> if there is an already an entry in the cache that has the same key as key.
        /// </returns>
        protected internal virtual bool AddCacheItem(string key, object value, CachePolicy cachePolicy)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            var cacheItem = new CacheItem(key, value, cachePolicy);
            return _cacheStore.TryAdd(key, cacheItem);
        }

        /// <summary>
        /// Inserts a cache entry into the cache overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="cachePolicy">An object that contains eviction details for the cache entry.</param>
        protected internal virtual void AddOrUpdateCacheItem(string key, object value, CachePolicy cachePolicy)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            CacheItem existing;
            if (_cacheStore.TryGetValue(key, out existing))
            {
                // update
                existing.Value = value;
                existing.UpdatePolicy(cachePolicy);
                existing.UpdateUsage();
            }
            else
            {
                // add
                existing = new CacheItem(key, value, cachePolicy);
                _cacheStore.TryAdd(key, existing);
            }
        }

        /// <summary>
        /// Gets the cache value for the specified key that is already in the dictionary or the new value for the key as returned by <paramref name="valueFactory"/>.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="valueFactory">The function used to generate a <see cref="CacheItem"/> to insert into cache.</param>
        /// <returns>
        /// The <see cref="CacheItem"/> for the key. This will be either the existing <see cref="CacheItem"/> for the key if the key is already in the dictionary, 
        /// or the new <see cref="CacheItem"/> for the key as returned by <paramref name="valueFactory"/> if the key was not in the dictionary.
        /// </returns>
        protected internal virtual CacheItem GetOrAddCacheItem(string key, Func<string, CacheItem> valueFactory)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (valueFactory == null)
                throw new ArgumentNullException("valueFactory");

            var item = _cacheStore.GetOrAdd(key, valueFactory);

            // refresh value when item is expired
            if (item.IsExpired())
            {
                CacheItem update = valueFactory(key);
                item.Value = update.Value;
                item.UpdatePolicy(update.CachePolicy);
            }

            item.UpdateUsage();

            return item;
        }

        /// <summary>
        /// Gets the cache value for the specified key
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <returns>The cache value for the specified key, if the entry exists; otherwise, <see langword="null"/>.</returns>
        protected internal virtual CacheItem GetCacheItem(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            CacheItem item;
            if (!_cacheStore.TryGetValue(key, out item))
                return null;

            if (item.IsExpired())
                return null;

            item.UpdateUsage();
            return item;
        }

        /// <summary>
        /// Removes a cache entry from the cache. 
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <returns>If the entry is found in the cache, the removed cache entry; otherwise, <see langword="null"/>.</returns>
        protected virtual CacheItem RemoveCacheItem(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            CacheItem item;
            return _cacheStore.TryRemove(key, out item) ? item : null;
        }
        #endregion

        #region Expiration Timer
        private static readonly TimeSpan _defaultTime = new TimeSpan(0, 0, 20);

        private readonly Timer _expirationTimer;
        private int _isRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheManager"/> class.
        /// </summary>
        public CacheManager()
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            TimeSpan span = _defaultTime - new TimeSpan(utcNow.Ticks % _defaultTime.Ticks);
            _expirationTimer = new Timer(ExpireCache, null, span.Ticks / 0x2710L, _defaultTime.Ticks / 0x2710L);
        }

        // run on background thread
        private void ExpireCache(object state)
        {
            if (Interlocked.Exchange(ref _isRunning, 1) != 0)
                return;

            try
            {
                var expired = _cacheStore
                  .Where(c => c.Value == null || c.Value.IsExpired())
                  .Select(c => c.Key)
                  .ToList();

                if (expired.Count == 0)
                    return;

                foreach (var key in expired)
                {
                    CacheItem item;
                    if (_cacheStore.TryRemove(key, out item))
                        item.RaiseExpiredCallback();
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
            }
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            _expirationTimer.Dispose();
        }
        #endregion

        #region Singleton
        private static readonly Lazy<CacheManager> _current = new Lazy<CacheManager>(() => new CacheManager());

        /// <summary>
        /// Gets a reference to the default Cache instance.
        /// </summary>
        public static CacheManager Cache
        {
            get { return _current.Value; }
        }
        #endregion

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            // copy values to new dictionary
            var dictionary = _cacheStore
              .Values
              .Where(i => i.IsExpired() == false)
              .ToDictionary(k => k.Key, v => v.Value);

            return dictionary.GetEnumerator();
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
