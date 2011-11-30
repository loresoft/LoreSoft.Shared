using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.IO
{
  public sealed class IsolatedStorageSettings
    : IDictionary<string, object>, IDictionary
  {
    private const string LocalSettingsPath = "__LocalSettings";
    private static readonly Lazy<IsolatedStorageSettings> s_appSettings = new Lazy<IsolatedStorageSettings>(() => new IsolatedStorageSettings(false));
    private static readonly Lazy<IsolatedStorageSettings> s_siteSettings = new Lazy<IsolatedStorageSettings>(() => new IsolatedStorageSettings(true));

    private readonly IsolatedStorageFile _appStore;
    private Dictionary<string, object> _settings;

    private IsolatedStorageSettings(bool useSiteSettings)
    {
      _appStore = useSiteSettings
          ? IsolatedStorageFile.GetUserStoreForSite()
          : IsolatedStorageFile.GetUserStoreForDomain();

      Reload();
    }

    public static IsolatedStorageSettings ApplicationSettings
    {
      get { return s_appSettings.Value; }
    }

    public static IsolatedStorageSettings SiteSettings
    {
      get { return s_siteSettings.Value; }
    }

    #region IDictionary Members

    public ICollection Keys
    {
      get { return _settings.Keys; }
    }

    public ICollection Values
    {
      get { return _settings.Values; }
    }

    bool IDictionary.IsFixedSize
    {
      get { return false; }
    }

    bool IDictionary.IsReadOnly
    {
      get { return false; }
    }

    object IDictionary.this[object key]
    {
      get
      {
        CheckNullKey(key);
        return ((IDictionary)_settings)[key];
      }
      set
      {
        CheckNullKey(key);
        ((IDictionary)_settings)[key] = value;
      }
    }

    int ICollection.Count
    {
      get { return _settings.Count; }
    }

    bool ICollection.IsSynchronized
    {
      get { return ((ICollection)_settings).IsSynchronized; }
    }

    object ICollection.SyncRoot
    {
      get { return ((ICollection)_settings).SyncRoot; }
    }

    void IDictionary.Add(object key, object value)
    {
      CheckNullKey(key);
      ((IDictionary)_settings).Add(key, value);
    }

    void IDictionary.Clear()
    {
      ((IDictionary)_settings).Clear();
    }

    bool IDictionary.Contains(object key)
    {
      CheckNullKey(key);
      return ((IDictionary)_settings).Contains(key);
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return ((IDictionary)_settings).GetEnumerator();
    }

    void IDictionary.Remove(object key)
    {
      CheckNullKey(key);
      ((IDictionary)_settings).Remove(key);
    }

    void ICollection.CopyTo(Array array, int index)
    {
      ((ICollection)_settings).CopyTo(array, index);
    }

    #endregion

    #region IDictionary<string,object> Members

    public int Count
    {
      get { return _settings.Count; }
    }

    public object this[string key]
    {
      get
      {
        CheckNullKey(key);
        return _settings[key];
      }
      set
      {
        CheckNullKey(key);
        _settings[key] = value;
      }
    }

    ICollection<string> IDictionary<string, object>.Keys
    {
      get { return _settings.Keys; }
    }

    ICollection<object> IDictionary<string, object>.Values
    {
      get { return _settings.Values; }
    }

    bool ICollection<KeyValuePair<string, object>>.IsReadOnly
    {
      get { return false; }
    }

    public void Add(string key, object value)
    {
      CheckNullKey(key);
      _settings.Add(key, value);
    }

    public bool Remove(string key)
    {
      CheckNullKey(key);
      return _settings.Remove(key);
    }

    bool IDictionary<string, object>.ContainsKey(string key)
    {
      CheckNullKey(key);
      return _settings.ContainsKey(key);
    }

    bool IDictionary<string, object>.TryGetValue(string key, out object value)
    {
      CheckNullKey(key);
      return _settings.TryGetValue(key, out value);
    }

    void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
    {
      ((ICollection<KeyValuePair<string, object>>)_settings).Add(item);
    }

    void ICollection<KeyValuePair<string, object>>.Clear()
    {
      _settings.Clear();
    }

    bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
    {
      return ((ICollection<KeyValuePair<string, object>>)_settings).Contains(item);
    }

    void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
      ((ICollection<KeyValuePair<string, object>>)_settings).CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
    {
      return ((ICollection<KeyValuePair<string, object>>)_settings).Remove(item);
    }

    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
    {
      return _settings.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _settings.GetEnumerator();
    }

    #endregion

    ~IsolatedStorageSettings()
    {
      if (_appStore == null)
        return;

      try
      {
        Save();
      }
      catch (Exception)
      { }

      _appStore.Dispose();
    }

    public void Clear()
    {
      _settings.Clear();
      Save();
    }

    public bool Contains(string key)
    {
      CheckNullKey(key);
      return _settings.ContainsKey(key);
    }

    private void Reload()
    {
      _settings = new Dictionary<string, object>();

      using (IsolatedStorageFileStream stream = _appStore.OpenFile(LocalSettingsPath, FileMode.OpenOrCreate))
      using (var reader = new StreamReader(stream))
      {
        if (stream.Length <= 0L)
        {
          return;
        }

        try
        {
          string line = reader.ReadLine();
          if (line.IsNullOrEmpty())
            return;

          var knownTypes = line.Split(new char[1])
            .Select(name => Type.GetType(name, false))
            .Where(item => item != null)
            .ToList();

          stream.Position = line.Length + Environment.NewLine.Length;
          var serializer = new DataContractSerializer(typeof(Dictionary<string, object>), knownTypes);
          _settings = (Dictionary<string, object>)serializer.ReadObject(stream);
        }
        catch (Exception)
        { }
      }
    }

    public void Save()
    {
      using (IsolatedStorageFileStream isolatedStorageFileStream = _appStore.OpenFile(LocalSettingsPath, FileMode.OpenOrCreate))
      using (var memoryStream = new MemoryStream())
      {
        var dictionary = new Dictionary<Type, bool>();
        var stringBuilder = new StringBuilder();
        foreach (object current in _settings.Values)
        {
          if (current == null)
            continue;

          Type type = current.GetType();
          if (type.IsPrimitive || type == typeof(string))
            continue;

          dictionary[type] = true;
          if (stringBuilder.Length > 0)
            stringBuilder.Append('\0');

          stringBuilder.Append(type.AssemblyQualifiedName);
        }
        stringBuilder.Append(Environment.NewLine);

        byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
        memoryStream.Write(bytes, 0, bytes.Length);

        var dataContractSerializer = new DataContractSerializer(typeof(Dictionary<string, object>), dictionary.Keys);
        dataContractSerializer.WriteObject(memoryStream, _settings);

        if (memoryStream.Length > _appStore.AvailableFreeSpace + isolatedStorageFileStream.Length)
          throw new IsolatedStorageException("Not enough space in Isolated Storage to save settings.");

        isolatedStorageFileStream.SetLength(0L);
        byte[] buffer = memoryStream.ToArray();
        isolatedStorageFileStream.Write(buffer, 0, buffer.Length);
      }
    }

    public bool TryGetValue<T>(string key, out T value)
    {
      CheckNullKey(key);
      object o;
      if (_settings.TryGetValue(key, out o))
      {
        value = (T)o;
        return true;
      }
      value = default(T);
      return false;
    }

    private void CheckNullKey(object key)
    {
      if (key == null)
      {
        throw new ArgumentNullException("key");
      }
    }
  }
}