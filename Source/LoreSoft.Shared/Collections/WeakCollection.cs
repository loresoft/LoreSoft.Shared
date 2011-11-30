using System;
using System.Collections.Generic;
using System.Linq;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Collections
{
    public sealed class WeakCollection<T>
    {
        private readonly List<WeakReference> _references;

        public WeakCollection()
        {
            Threshold = 70;
            _references = new List<WeakReference>();
        }

        /// <summary>
        /// A percentage value between 0 and 100 inclusive.  When the percentage of Values collected is greater than
        /// or equal to this percentage then a collection will occur and the underlying table structure
        /// will be shrunk to only valid values
        /// </summary>
        public int Threshold { get; set; }

        public List<T> Items
        {
            get
            {
                // copy to structure to prevent GC
                return _references.Select(p => Tuple.Create(p.Target))
                  .Where(t => t.Item1 != null)
                  .Select(t => (T)t.Item1)
                  .ToList();
            }
        }

        public void Add(T value)
        {
            MaybeCleanup();
            _references.Add(new WeakReference(value));
        }

        public bool Remove(T value)
        {
            MaybeCleanup();

            WeakReference remove = _references
              .FirstOrDefault(r => Equals(r.Target, value));

            if (remove == null)
                return false;

            return _references.Remove(remove);
        }

        public void Cleanup()
        {
            _references.RemoveAll(p => !p.IsAlive);
        }

        private void MaybeCleanup()
        {
            if (!ShouldCleanup())
                return;

            Cleanup();
        }

        private bool ShouldCleanup()
        {
            int countCollected = _references
              .Where(x => !x.IsAlive)
              .Count();

            double diff = countCollected / (double)(_references.Count);
            int percent = (int)(100 * diff);

            return percent >= Threshold.Fit(0, 100);
        }

    }
}