using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using LoreSoft.Shared.ComponentModel;

namespace LoreSoft.Shared.Controls
{
  [DataContract]
  public class DataGridColumnMetadata : NotificationBase
  {
    private int _index;
    [DataMember]
    public int Index
    {
      get { return _index; }
      set
      {
        if (_index == value)
          return;

        _index = value;
        RaisePropertyChanged(() => Index);
      }
    }

    private int _displayIndex;
    [DataMember]
    public int DisplayIndex
    {
      get { return _displayIndex; }
      set
      {
        if (_displayIndex == value)
          return;

        _displayIndex = value;
        RaisePropertyChanged(() => DisplayIndex);
      }
    }

    private double _width;
    [DataMember]
    public double Width
    {
      get { return _width; }
      set
      {
        if (_width == value)
          return;

        _width = value;
        RaisePropertyChanged(() => Width);
      }
    }

    private Visibility _visibility;
    [DataMember]
    public Visibility Visibility
    {
      get { return _visibility; }
      set
      {
        if (_visibility == value)
          return;

        _visibility = value;
        RaisePropertyChanged(() => Visibility);
      }
    }

    private string _header;
    [DataMember]
    public string Header
    {
      get { return _header; }
      set
      {
        if (_header == value)
          return;

        _header = value;
        RaisePropertyChanged(() => Header);
      }
    }

    private ListSortDirection? _sortDirection;
    [DataMember]
    public ListSortDirection? SortDirection
    {
      get { return _sortDirection; }
      set
      {
        if (_sortDirection == value)
          return;

        _sortDirection = value;
        RaisePropertyChanged(() => SortDirection);
      }
    }

    public bool IsVisible
    {
      get { return Visibility == Visibility.Visible; }
      set
      {
        if (Visibility == Visibility.Visible && value)
          return;
        if (Visibility != Visibility.Visible && !value)
          return;

        Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        RaisePropertyChanged(() => IsVisible);
        RaisePropertyChanged(() => Visibility);
      }
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = Index;
        result = (result * 397) ^ DisplayIndex;
        result = (result * 397) ^ Width.GetHashCode();
        result = (result * 397) ^ Visibility.GetHashCode();
        result = (result * 397) ^ (Header != null ? Header.GetHashCode() : 0);
        result = (result * 397) ^ (SortDirection != null ? SortDirection.GetHashCode() : -1);
        return result;
      }
    }
  }
}