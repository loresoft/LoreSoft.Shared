using System;
using System.Collections.Generic;
using System.Linq;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Collections
{
    public sealed class WeakDictionary<TKey, TValue>
    {
        private Dictionary<TKey, WeakReference> _map;

        public List<Tuple<TKey, TValue>> Pairs
        {
            get
            {
                return _map
                        .Select(p => Tuple.Create(p.Key, p.Value.Target))
                        .Where(t => t.Item2 != null)
                        .Select(t => Tuple.Create(t.Item1, (TValue)t.Item2))
                        .ToList();
            }
        }

        public List<TValue> Values
        {
            get { return Pairs.Select(x => x.Item2).ToList(); }
        }

        /// <summary>
        /// A percentage value between 0 and 100 inclusive.  When the percentage of Values collected is greater than
        /// or equal to this percentage then a collection will occur and the underlying table structure
        /// will be shrunk to only valid values
        /// </summary>
        public int Threshold { get; set; }

        public WeakDictionary()
            : this(EqualityComparer<TKey>.Default)
        {
        }

        public WeakDictionary(IEqualityComparer<TKey> comparer)
        {
            Threshold = 70;
            _map = new Dictionary<TKey, WeakReference>(comparer);
        }

        public void Add(TKey key, TValue value)
        {
            MaybeCleanup();
            _map.Add(key, new WeakReference(value));
        }

        public void Put(TKey key, TValue value)
        {
            MaybeCleanup();
            _map[key] = new WeakReference(value);
        }

        public bool Remove(TKey key)
        {
            MaybeCleanup();
            return _map.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            WeakReference reference;

            if (!_map.TryGetValue(key, out reference))
                return false;

            var target = reference.Target;
            if (target == null)
                return false;

            value = (TValue)target;
            return true;
        }

        public void Cleanup()
        {
            var old = _map;
            _map = new Dictionary<TKey, WeakReference>(old.Comparer);
            foreach (var cur in old.Where(p => p.Value.IsAlive))
                _map.Add(cur.Key, cur.Value);
        }

        private void MaybeCleanup()
        {
            if (!ShouldCleanup())
                return;
            Cleanup();
        }

        private bool ShouldCleanup()
        {
            var countCollected = (double)(_map.Values.Where(x => !x.IsAlive).Count());
            var diff = countCollected / (double)(_map.Count);
            var percent = (int)(100 * diff);
            return percent >= Threshold.Fit(0, 100);
        }
    }
}
