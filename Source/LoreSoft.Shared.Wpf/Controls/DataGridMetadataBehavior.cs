using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Threading;
using LoreSoft.Shared.Extensions;
using LoreSoft.Shared.Security;
using LoreSoft.Shared.Threading;

namespace LoreSoft.Shared.Controls
{
  public class DataGridMetadataBehavior : Behavior<DataGrid>
  {
    private readonly DelayedAction _updateAction;
    private readonly BusyMonitor _updateMonitor;
    private readonly BusyMonitor _applyMonitor;
    private DataGridMetadata _defaultMetadata;
    private bool _isLoaded;

    private readonly DependencyPropertyDescriptor _widthProperty;
    private readonly DependencyPropertyDescriptor _displayIndexProperty;
    private readonly DependencyPropertyDescriptor _visibilityProperty;
    private readonly DependencyPropertyDescriptor _sortDirectionProperty;

    public DataGridMetadataBehavior()
    {
      _sortDirectionProperty = DependencyPropertyDescriptor.FromProperty(DataGridColumn.SortDirectionProperty, typeof(DataGridColumn));
      _visibilityProperty = DependencyPropertyDescriptor.FromProperty(DataGridColumn.VisibilityProperty, typeof(DataGridColumn));
      _displayIndexProperty = DependencyPropertyDescriptor.FromProperty(DataGridColumn.DisplayIndexProperty, typeof(DataGridColumn));
      _widthProperty = DependencyPropertyDescriptor.FromProperty(DataGridColumn.WidthProperty, typeof(DataGridColumn));

      _updateAction = new DelayedAction(UpdateMetadata, TimeSpan.FromMilliseconds(500));
      _updateMonitor = new BusyMonitor();
      _applyMonitor = new BusyMonitor();
    }

    #region Metadata
    public DataGridMetadata Metadata
    {
      get { return (DataGridMetadata)GetValue(MetadataProperty); }
      set { SetValue(MetadataProperty, value); }
    }

    public static readonly DependencyProperty MetadataProperty =
        DependencyProperty.Register(
            "Metadata",
            typeof(DataGridMetadata),
            typeof(DataGridMetadataBehavior),
            new PropertyMetadata(null, OnMetadataChanged));

    private static void OnMetadataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as DataGridMetadataBehavior;
      if (source == null)
        return;

      source.OnMetadataChanged(e);
    }

    protected virtual void OnMetadataChanged(DependencyPropertyChangedEventArgs e)
    {
      // don't apply if updating metadata
      if (_updateMonitor.IsBusy || !_isLoaded)
        return;

      if (e.NewValue == null && e.OldValue != null)
        Reset();
      else
        ApplyMetadata(e.NewValue as DataGridMetadata);
    }
    #endregion

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.Loaded += OnDataGridLoaded;
      AssociatedObject.Columns.CollectionChanged += OnColumnsCollectionChanged;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();

      AssociatedObject.LayoutUpdated -= OnDataGridUpdated;
      AssociatedObject.Columns.CollectionChanged -= OnColumnsCollectionChanged;

      _updateAction.Dispose();
    }

    private void OnDataGridLoaded(object sender, RoutedEventArgs e)
    {
      // capture default values from xaml before applying any
      _defaultMetadata = CreateMetadata();
      _isLoaded = true;
      AssociatedObject.Loaded -= OnDataGridLoaded;

      if (Metadata == null)
        UpdateMetadata();
      else
        ApplyMetadata(Metadata);
    }

    private void OnColumnsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      var newItems = e.NewItems == null
        ? Enumerable.Empty<DataGridColumn>()
        : e.NewItems.OfType<DataGridColumn>();

      var oldItems = e.OldItems == null
        ? Enumerable.Empty<DataGridColumn>()
        : e.OldItems.OfType<DataGridColumn>();

      switch (e.Action)
      {
        case NotifyCollectionChangedAction.Add:
          AddHandlers(newItems);
          break;
        case NotifyCollectionChangedAction.Remove:
          RemoveHandlers(oldItems);
          break;
        case NotifyCollectionChangedAction.Replace:
          RemoveHandlers(oldItems);
          AddHandlers(newItems);
          break;
        case NotifyCollectionChangedAction.Move:
          break;
        case NotifyCollectionChangedAction.Reset:
          AddHandlers(AssociatedObject.Columns);
          break;
      }
    }

    private void AddHandlers(IEnumerable<DataGridColumn> newItems)
    {
      if (newItems == null)
        return;

      foreach (var column in newItems)
      {
        _widthProperty.AddValueChanged(column, OnDataGridUpdated);
        _displayIndexProperty.AddValueChanged(column, OnDataGridUpdated);
        _visibilityProperty.AddValueChanged(column, OnDataGridUpdated);
        _sortDirectionProperty.AddValueChanged(column, OnDataGridUpdated);
      }
    }

    private void RemoveHandlers(IEnumerable<DataGridColumn> oldItems)
    {
      if (oldItems == null)
        return;

      foreach (var column in oldItems)
      {
        _widthProperty.RemoveValueChanged(column, OnDataGridUpdated);
        _displayIndexProperty.RemoveValueChanged(column, OnDataGridUpdated);
        _visibilityProperty.RemoveValueChanged(column, OnDataGridUpdated);
        _sortDirectionProperty.RemoveValueChanged(column, OnDataGridUpdated);
      }
    }

    private void OnDataGridUpdated(object sender, EventArgs e)
    {
      // ignore update if busy appling metadata
      if (_applyMonitor.IsBusy || !_isLoaded)
        return;

      // delay call to smooth out rapid calls
      _updateAction.Trigger();
    }

    private void Reset()
    {
      if (!_isLoaded)
        return;

      if (_defaultMetadata == null)
        return;

      ApplyMetadata(_defaultMetadata);
      Dispatcher.BeginInvoke(new Action(UpdateMetadata));

      return;
    }

    private void ApplyMetadata(DataGridMetadata metadata)
    {
      if (metadata == null)
        return;

      Debug.WriteLine("ApplyMetadata()");
      using (_applyMonitor.Enter())
      {
        int columnCount = AssociatedObject.Columns.Count;
        var collectionView = AssociatedObject.Items as ICollectionView;
        bool clearSortDescriptions = true;

        foreach (var columnMetadata in metadata.ColumnMetadata)
        {
          if (columnMetadata.Index >= columnCount)
            continue;

          var column = AssociatedObject.Columns[columnMetadata.Index];

          if (columnMetadata.DisplayIndex >= 0 && columnMetadata.DisplayIndex < columnCount)
            column.DisplayIndex = columnMetadata.DisplayIndex;

          column.Visibility = columnMetadata.Visibility;
          column.SortDirection = columnMetadata.SortDirection;
          column.Width = new DataGridLength(columnMetadata.Width);

          if (collectionView == null || column.SortDirection == null)
            continue;

          if (clearSortDescriptions)
          {
            collectionView.SortDescriptions.Clear();
            clearSortDescriptions = false;
          }

          var sortDescription = new SortDescription(column.SortMemberPath, column.SortDirection.Value);
          collectionView.SortDescriptions.Add(sortDescription);
        }
      }

    }

    private void UpdateMetadata()
    {
      var metadata = CreateMetadata();
      if (Metadata != null && Metadata == metadata)
        return;

      Debug.WriteLine("UpdateMetadata()");
      using (_updateMonitor.Enter())
        Metadata = metadata;
    }

    private DataGridMetadata CreateMetadata()
    {
      var metadata = new DataGridMetadata();

      for (int index = 0; index < AssociatedObject.Columns.Count; index++)
      {
        var column = AssociatedObject.Columns[index];
        var columnMetadata = new DataGridColumnMetadata
        {
          Index = index,
          DisplayIndex = column.DisplayIndex,
          Visibility = column.Visibility,
          Width = column.ActualWidth,
          SortDirection = column.SortDirection,
          Header = column.Header == null ? "Column " + index : column.Header.ToString()
        };
        metadata.ColumnMetadata.Add(columnMetadata);
      }

      return metadata;
    }
  }
}
