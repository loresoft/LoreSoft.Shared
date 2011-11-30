using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using LoreSoft.Shared.ComponentModel;

namespace LoreSoft.Shared.Controls
{
  [DataContract]
  public class DataGridMetadata : NotificationBase, IEquatable<DataGridMetadata>
  {
    public DataGridMetadata()
    {
      _columnMetadata = new ObservableCollection<DataGridColumnMetadata>();
    }

    private ObservableCollection<DataGridColumnMetadata> _columnMetadata;
    [DataMember]
    public ObservableCollection<DataGridColumnMetadata> ColumnMetadata
    {
      get { return _columnMetadata; }
      set
      {
        if (_columnMetadata == value)
          return;

        _columnMetadata = value;
        RaisePropertyChanged(() => ColumnMetadata);
      }
    }

    public bool Equals(DataGridMetadata other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;

      return other.GetHashCode() == GetHashCode();
    }

    public override bool Equals(object other)
    {
      if (ReferenceEquals(null, other))
        return false;
      if (ReferenceEquals(this, other))
        return true;
      if (!(other is DataGridMetadata))
        return false;

      return Equals((DataGridMetadata)other);
    }

    public static bool operator ==(DataGridMetadata left, DataGridMetadata right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(DataGridMetadata left, DataGridMetadata right)
    {
      return !Equals(left, right);
    }

    public override int GetHashCode()
    {
      return ColumnMetadata.Aggregate(1, (c, v) => (c * 397) ^ v.GetHashCode());
    }
  }
}