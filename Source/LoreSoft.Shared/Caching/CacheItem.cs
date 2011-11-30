using System;
using System.Diagnostics;
using System.Threading;

namespace LoreSoft.Shared.Caching
{
    /// <summary>
    /// Represents an individual cache entry in the cache. 
    /// </summary>
    [DebuggerDisplay("Key: {Key}")]
    public class CacheItem
    {
        /// <summary>
        /// Initializes a new <see cref="CacheItem"/> instance using the specified key and value for the cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for a <see cref="CacheItem"/> entry.</param>
        /// <param name="value">The data for a <see cref="CacheItem"/> entry.</param>
        public CacheItem(string key, object value)
            : this(key, value, null)
        { }

        /// <summary>
        /// Initializes a new <see cref="CacheItem"/> instance using the specified key and value for the cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for a <see cref="CacheItem"/> entry.</param>
        /// <param name="value">The data for a <see cref="CacheItem"/> entry.</param>
        /// <param name="cachePolicy">An object that contains eviction details for the <see cref="CacheItem"/>.</param>
        public CacheItem(string key, object value, CachePolicy cachePolicy)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (value == null)
                throw new ArgumentNullException("value");

            _key = key;
            Value = value;
            CachePolicy = cachePolicy ?? new CachePolicy();

            _created = DateTimeOffset.UtcNow;

            if (CachePolicy.SlidingExpiration > TimeSpan.Zero)
                _absoluteExpiration = Created + CachePolicy.SlidingExpiration;
            else
                _absoluteExpiration = CachePolicy.AbsoluteExpiration;
        }

        private readonly string _key;

        /// <summary>
        /// Gets or sets a unique identifier for a <see cref="CacheItem"/> instance. 
        /// </summary>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets or sets the data for a <see cref="CacheItem"/> instance.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CachePolicy"/> that contains eviction details for a <see cref="CacheItem"/> instance.
        /// </summary>
        public CachePolicy CachePolicy { get; private set; }

        private readonly DateTimeOffset _created;

        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> of when this <see cref="CacheItem"/> was created, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        internal DateTimeOffset Created
        {
            get { return _created; }
        }

        private DateTimeOffset _absoluteExpiration;

        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> when this <see cref="CacheItem"/> will expire, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        internal DateTimeOffset AbsoluteExpiration
        {
            get { return _absoluteExpiration; }
        }

        private DateTimeOffset _lastUpdateUsage;

        /// <summary>
        /// Gets a <see cref="DateTimeOffset"/> when this <see cref="CacheItem"/> usage was last updated, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        internal DateTimeOffset LastUpdateUsage
        {
            get { return _lastUpdateUsage; }
        }

        /// <summary>
        /// Determines whether this <see cref="CacheItem"/> instance can expire.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance can expire; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanExpire()
        {
            return (AbsoluteExpiration < DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// Called to update the sliding expiration of this <see cref="CacheItem"/> instance.
        /// </summary>
        internal void UpdateUsage()
        {
            _lastUpdateUsage = DateTimeOffset.UtcNow;
            if (CachePolicy.SlidingExpiration <= TimeSpan.Zero)
                return;

            _absoluteExpiration = LastUpdateUsage + CachePolicy.SlidingExpiration;
        }

        /// <summary>
        /// Updates the <see cref="CachePolicy"/> for this <see cref="CacheItem"/>.
        /// </summary>
        /// <param name="cachePolicy">The cache policy.</param>
        internal void UpdatePolicy(CachePolicy cachePolicy)
        {
            CachePolicy = cachePolicy ?? new CachePolicy();

            _lastUpdateUsage = DateTimeOffset.UtcNow;

            if (CachePolicy.SlidingExpiration > TimeSpan.Zero)
                _absoluteExpiration = LastUpdateUsage + CachePolicy.SlidingExpiration;
            else
                _absoluteExpiration = CachePolicy.AbsoluteExpiration.UtcDateTime;
        }

        /// <summary>
        /// Determines whether this <see cref="CacheItem"/> instance is expired.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is expired; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsExpired()
        {
            // value not allowed to be null
            return Value == null ||
              (CanExpire() && AbsoluteExpiration < DateTimeOffset.UtcNow);
        }

        /// <summary>
        /// Raises the expired callback asynchronously on the <see cref="ThreadPool"/>.
        /// </summary>
        internal void RaiseExpiredCallback()
        {
            var handle = CachePolicy.ExpiredCallback;
            if (handle == null)
                return;

            // run async
            ThreadPool.QueueUserWorkItem(s => handle(this));
        }

    }
}