using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using LoreSoft.Shared.Extensions;
using LoreSoft.Shared.Threading;

namespace LoreSoft.Shared.Configuration
{
    public interface IDynamicSettings
      : IDictionary<string, object>
    {
        void Save();
        void Load();
    }

    public class DynamicSettings
      : DynamicObject,
      IDynamicSettings,
      IDictionary,
      INotifyPropertyChanged
    {
        private readonly SettingsDictionary _settings;
        private readonly DelayedAction _saveAction;
        private readonly object _settingsLock;

        public DynamicSettings()
            : this("Settings.Config")
        {
        }

        public DynamicSettings(string settingFile)
        {
            _settingFile = settingFile;
            _settings = new SettingsDictionary();
            _saveAction = new DelayedAction(Save, TimeSpan.FromMilliseconds(500));
            _settingsLock = new object();

            Load();
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<MemberBinderNotFoundEventArgs> MemberBinderNotFound;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler == null)
                return;

            handler(this, e);
        }
        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnMemberBinderNotFound(MemberBinderNotFoundEventArgs e)
        {
            var handler = MemberBinderNotFound;
            if (handler == null)
                return;

            handler(this, e);
        }
        private void OnMemberBinderNotFound(string propertyName)
        {
            OnMemberBinderNotFound(new MemberBinderNotFoundEventArgs(propertyName));
        }

        #endregion

        private readonly string _settingFile;
        public string SettingFile
        {
            get { return _settingFile; }
        }

        public int Count
        {
            get { return _settings.Count; }
        }
        public object this[string key]
        {
            get { return _settings[key]; }
            set { SetItem(key, value); }
        }
        public ICollection<string> Keys
        {
            get { return _settings.Keys; }
        }
        public ICollection<object> Values
        {
            get { return _settings.Values; }
        }

        public void Add(string key, object value)
        {
            AddItem(key, value);
        }
        public void Clear()
        {
            ClearItems();
        }
        public bool ContainsKey(string key)
        {
            return _settings.ContainsKey(key);
        }
        public bool ContainsValue(object value)
        {
            return _settings.ContainsValue(value);
        }
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _settings.GetEnumerator();
        }
        public bool Remove(string key)
        {
            return RemoveItem(key);
        }
        public bool TryGetValue(string key, out object value)
        {
            return _settings.TryGetValue(key, out value);
        }

        #region ICollection<KeyValuePair<string, object>>
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string, object>>)_settings).IsReadOnly; }
        }
        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> keyValuePair)
        {
            AddItem(keyValuePair.Key, keyValuePair.Value);
        }
        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> keyValuePair)
        {
            return ((ICollection<KeyValuePair<string, object>>)_settings).Contains(keyValuePair);
        }
        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int index)
        {
            ((ICollection<KeyValuePair<string, object>>)_settings).CopyTo(array, index);
        }
        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> keyValuePair)
        {
            return RemoveItem(keyValuePair.Key);
        }
        #endregion

        #region IDictionary<string, object>
        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return ((IDictionary<string, object>)_settings).Keys; }
        }
        ICollection<object> IDictionary<string, object>.Values
        {
            get { return ((IDictionary<string, object>)_settings).Values; }
        }
        #endregion

        #region ICollection
        bool ICollection.IsSynchronized
        {
            get { return ((IDictionary)_settings).IsFixedSize; }
        }
        object ICollection.SyncRoot
        {
            get { return ((IDictionary)_settings).IsFixedSize; }
        }
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_settings).CopyTo(array, index);

        }
        #endregion

        #region IDictionary
        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary)_settings).IsFixedSize; }
        }
        bool IDictionary.IsReadOnly
        {
            get { return ((IDictionary)_settings).IsReadOnly; }
        }
        object IDictionary.this[object key]
        {
            get { return ((IDictionary)_settings)[key]; }
            set { SetItem((string)key, value); }
        }
        ICollection IDictionary.Keys
        {
            get { return ((IDictionary)_settings).Keys; }
        }
        ICollection IDictionary.Values
        {
            get { return ((IDictionary)_settings).Values; }
        }
        void IDictionary.Add(object key, object value)
        {
            AddItem((string)key, value);
        }
        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)_settings).Contains(key);
        }
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)_settings).GetEnumerator();
        }
        void IDictionary.Remove(object key)
        {
            RemoveItem((string)key);
        }
        #endregion

        #region IEnumerable
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)_settings).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_settings).GetEnumerator();
        }
        #endregion

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            bool isFound = TryGetValue(binder.Name, out result);

            if (!isFound)
                OnMemberBinderNotFound(binder.Name);

            // always return true
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetItem(binder.Name, value);
            return true;
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            bool isFound = RemoveItem(binder.Name);

            if (!isFound)
                OnMemberBinderNotFound(binder.Name);

            // always return true
            return true;
        }

        protected virtual void ClearItems()
        {
            var keys = _settings.Keys.ToList();
            _settings.Clear();
            foreach (string key in keys)
                OnPropertyChanged(key);

            _saveAction.Trigger();
        }

        protected virtual void AddItem(string key, object item)
        {
            _settings.Add(key, item);
            OnPropertyChanged(key);

            _saveAction.Trigger();
        }

        protected virtual bool RemoveItem(string key)
        {
            var isRemoved = _settings.Remove(key);
            if (isRemoved)
            {
                OnPropertyChanged(key);
                _saveAction.Trigger();
            }

            return isRemoved;
        }

        protected virtual void SetItem(string key, object item)
        {
            _settings[key] = item;
            OnPropertyChanged(key);
            _saveAction.Trigger();
        }

        public virtual void Save()
        {
            lock (_settingsLock)
            {
                // cancel pending save
                _saveAction.Cancel();

                using (var storage = GetStorage())
                using (var stream = new IsolatedStorageFileStream(SettingFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, storage))
                {
                    // work around issue with isolated storage, rewrite file instead of delete and create
                    stream.SetLength(0L);
                    _settings.Serialize(stream);
                    stream.Flush();
                }
            }
        }

        public virtual void Load()
        {
            lock (_settingsLock)
            {
                using (var storage = GetStorage())
                {
                    if (!storage.FileExists(SettingFile))
                        return;

                    using (var stream = new IsolatedStorageFileStream(SettingFile, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read, storage))
                        _settings.Deserialize(stream);
                }
            }
        }

        private IsolatedStorageFile GetStorage()
        {
#if SILVERLIGHT
            return IsolatedStorageFile.GetUserStoreForApplication();
#else
            return IsolatedStorageFile.GetUserStoreForAssembly();
#endif
        }

        [CollectionDataContract(Name = "settings", ItemName = "setting", KeyName = "key", ValueName = "value")]
        public class SettingsDictionary : Dictionary<string, object>
        { }
    }

    public class MemberBinderNotFoundEventArgs : EventArgs
    {
        public MemberBinderNotFoundEventArgs(string name)
        {
            _name = name;
        }

        private readonly string _name;

        public string Name
        {
            get { return _name; }
        }
    }
}
