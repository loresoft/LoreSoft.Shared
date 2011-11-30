using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LoreSoft.Shared.Controls
{
  public class DataGridSynchronizeColumnsBehavior : Behavior<DataGrid>
  {
    #region Grid
    public Grid Grid
    {
      get { return (Grid)GetValue(GridProperty); }
      set { SetValue(GridProperty, value); }
    }

    public static readonly DependencyProperty GridProperty =
        DependencyProperty.Register(
            "Grid",
            typeof(Grid),
            typeof(DataGridSynchronizeColumnsBehavior),
            new PropertyMetadata(null, OnGridChanged));

    private static void OnGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as DataGridSynchronizeColumnsBehavior;
      if (source == null)
        return;

      source.OnGridChanged(e);
    }

    protected virtual void OnGridChanged(DependencyPropertyChangedEventArgs e)
    {

    }
    #endregion

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.LayoutUpdated += OnDataGridLayoutUpdated;
    }


    private void OnDataGridLayoutUpdated(object sender, EventArgs e)
    {
      SynchronizeColumns();
    }


    private void SynchronizeColumns()
    {
      if (AssociatedObject == null || Grid == null)
        return;

      if (AssociatedObject.Columns.Count == Grid.ColumnDefinitions.Count)
        for (int i = 0; i < AssociatedObject.Columns.Count; i++)
          Grid.ColumnDefinitions[i].Width = new GridLength(AssociatedObject.Columns[i].ActualWidth);
    }
  }
}
