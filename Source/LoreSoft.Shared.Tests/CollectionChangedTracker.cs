using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LoreSoft.Shared.Tests
{
  public class CollectionChangedTracker
  {
    private readonly List<NotifyCollectionChangedEventArgs> _eventList = new List<NotifyCollectionChangedEventArgs>();

    public CollectionChangedTracker(INotifyCollectionChanged collection)
    {
      collection.CollectionChanged += OnCollectionChanged;
    }

    public IEnumerable<NotifyCollectionChangedAction> ActionsFired { get { return this._eventList.Select(e => e.Action); } }
    public IEnumerable<NotifyCollectionChangedEventArgs> NotifyEvents { get { return this._eventList; } }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this._eventList.Add(e);
    }
  }
}
