using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using LoreSoft.Shared.ComponentModel;
using LoreSoft.Shared.Threading;

namespace LoreSoft.Shared.Controls
{
  public class GridSplitterMetadataBehavior : Behavior<GridSplitter>
  {
    private readonly DelayedAction _updateAction;
    private readonly BusyMonitor _updateMonitor;
    private readonly BusyMonitor _applyMonitor;
    private bool _isLoaded;

    public GridSplitterMetadataBehavior()
    {
      _updateAction = new DelayedAction(UpdateMetadata, TimeSpan.FromMilliseconds(800));
      _updateMonitor = new BusyMonitor();
      _applyMonitor = new BusyMonitor();
    }

    #region Metadata
    public GridMetadata Metadata
    {
      get { return (GridMetadata)GetValue(MetadataProperty); }
      set { SetValue(MetadataProperty, value); }
    }

    public static readonly DependencyProperty MetadataProperty =
        DependencyProperty.Register(
            "Metadata",
            typeof(GridMetadata),
            typeof(GridSplitterMetadataBehavior),
            new PropertyMetadata(null, OnMetadataChanged));

    private static void OnMetadataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as GridSplitterMetadataBehavior;
      if (source == null)
        return;

      source.OnMetadataChanged(e);
    }

    protected virtual void OnMetadataChanged(DependencyPropertyChangedEventArgs e)
    {
      // prevent apply if update is busy
      if (_updateMonitor.IsBusy || !_isLoaded)
        return;

      ApplyMetadata();
    }
    #endregion

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      _isLoaded = true;
      AssociatedObject.Loaded -= OnLoaded;

      var grid = AssociatedObject.Parent as Grid;
      if (grid == null)
        return;

      if (Metadata == null)
        UpdateMetadata();
      else
        ApplyMetadata();

      // listen for changes
      var columnProperty = DependencyPropertyDescriptor.FromProperty(ColumnDefinition.WidthProperty, typeof(ColumnDefinition));
      var rowProperty = DependencyPropertyDescriptor.FromProperty(RowDefinition.HeightProperty, typeof(RowDefinition));

      foreach (var definition in grid.ColumnDefinitions)
        columnProperty.AddValueChanged(definition, OnDefinitionChange);
      foreach (var definition in grid.RowDefinitions)
        rowProperty.AddValueChanged(definition, OnDefinitionChange);
    }

    private void OnDefinitionChange(object sender, EventArgs e)
    {
      // prevent update if apply is busy
      if (_applyMonitor.IsBusy)
        return;

      _updateAction.Trigger();
    }
    
    private void ApplyMetadata()
    {
      if (Metadata == null)
        return;

      var grid = AssociatedObject.Parent as Grid;
      if (grid == null)
        return;

      int splitterColumn = Grid.GetColumn(AssociatedObject);
      int splitterRow = Grid.GetRow(AssociatedObject);

      using (_applyMonitor.Enter())
      {
        for (int index = 0; index < Metadata.ColumnWidths.Count; index++)
        {
          // don't size column that the splitter is in
          if (index == splitterColumn)
            continue;

          var columnWidth = Metadata.ColumnWidths[index];
          var columnDefinition = grid.ColumnDefinitions[index];

          columnDefinition.Width = new GridLength(columnWidth, GridUnitType.Star);
        }

        for (int index = 0; index < Metadata.RowHeights.Count; index++)
        {
          // don't size row that the splitter is in
          if (index == splitterRow)
            continue;

          var rowHeight = Metadata.RowHeights[index];
          var rowDefinition = grid.RowDefinitions[index];

          rowDefinition.Height = new GridLength(rowHeight, GridUnitType.Star);
        } 
      }
    }

    private void UpdateMetadata()
    {
      var metadata = CreateMetadata();
      if (Metadata != null && Metadata == metadata)
        return;

      using (_updateMonitor.Enter())
        Metadata = metadata;
    }

    public GridMetadata CreateMetadata()
    {
      var metadata = new GridMetadata();
      var grid = AssociatedObject.Parent as Grid;
      if (grid == null)
        return metadata;

      double totalWidth = grid.ActualWidth;
      double totalHeight = grid.ActualHeight;
      int splitterColumn = Grid.GetColumn(AssociatedObject);
      int splitterRow = Grid.GetRow(AssociatedObject);

      for (int index = 0; index < grid.ColumnDefinitions.Count; index++)
      {
        var definition = grid.ColumnDefinitions[index];
        double width = definition.ActualWidth;
        if (splitterColumn != index)
          width = (width / totalWidth) * 100;

        metadata.ColumnWidths.Add(width);
      }

      for (int index = 0; index < grid.RowDefinitions.Count; index++)
      {
        var definition = grid.RowDefinitions[index];
        double height = definition.ActualHeight;
        if (splitterRow != index)
          height = (height / totalHeight) * 100;

        metadata.RowHeights.Add(height);
      }

      return metadata;
    }
  }

  [DataContract]
  public class GridMetadata : NotificationBase, IEquatable<GridMetadata> 
  {
    public GridMetadata()
    {
      ColumnWidths = new ObservableCollection<double>();
      RowHeights = new ObservableCollection<double>();
    }

    [DataMember]
    public ObservableCollection<double> ColumnWidths { get; set; }
    
    [DataMember]
    public ObservableCollection<double> RowHeights { get; set; }

    public bool Equals(GridMetadata other)
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
      if (!(other is GridMetadata))
        return false;

      return Equals((GridMetadata)other);
    }

    public static bool operator ==(GridMetadata left, GridMetadata right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(GridMetadata left, GridMetadata right)
    {
      return !Equals(left, right);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int result = ColumnWidths.Aggregate(1, (c, v) => (c * 397) ^ v.GetHashCode());
        result = (result * 397) ^ RowHeights.Aggregate(1, (c, v) => (c * 397) ^ v.GetHashCode());
        return result;
      }
    }
  }
}
