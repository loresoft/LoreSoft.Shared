using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace LoreSoft.Shared.Controls
{
  public class DataGridDynamicColumnsBehavior : Behavior<DataGrid>
  {
    private WeakEventListener<DataGridDynamicColumnsBehavior, object, NotifyCollectionChangedEventArgs> _weakEventListener;

    #region Columns
    public IEnumerable<DataGridColumn> Columns
    {
      get { return (IEnumerable<DataGridColumn>)GetValue(ColumnsProperty); }
      set { SetValue(ColumnsProperty, value); }
    }

    public static readonly DependencyProperty ColumnsProperty =
      DependencyProperty.Register(
        "Columns",
        typeof(IEnumerable<DataGridColumn>),
        typeof(DataGridDynamicColumnsBehavior),
        new PropertyMetadata(OnColumnsChanged));

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as DataGridDynamicColumnsBehavior;
      if (source == null)
        return;

      source.OnColumnsChanged(e);
    }


    #endregion

    #region StartIndex
    public int StartIndex
    {
      get { return (int)GetValue(StartIndexProperty); }
      set { SetValue(StartIndexProperty, value); }
    }

    public static readonly DependencyProperty StartIndexProperty =
        DependencyProperty.Register(
            "StartIndex",
            typeof(int),
            typeof(DataGridDynamicColumnsBehavior),
            new PropertyMetadata(0, OnStartIndexChanged));

    private static void OnStartIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as DataGridDynamicColumnsBehavior;
      if (source == null)
        return;

      source.OnStartIndexChanged(e);
    }

    protected virtual void OnStartIndexChanged(DependencyPropertyChangedEventArgs e)
    {
      UpdateColumns();
    }
    #endregion

    protected override void OnAttached()
    {
      base.OnAttached();
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      Cleanup();
    }

    private void Cleanup()
    {      
      if (_weakEventListener != null)
        _weakEventListener.Detach();
      
      _weakEventListener = null;
    }
    
    protected virtual void OnColumnsChanged(DependencyPropertyChangedEventArgs e)
    {
      var oldColumns = e.OldValue as INotifyCollectionChanged;
      var newColumns = e.NewValue as INotifyCollectionChanged;

      if (oldColumns != null && _weakEventListener != null)
      {
        _weakEventListener.Detach();
        _weakEventListener = null;
      }
      if (newColumns != null)
      {
        _weakEventListener = new WeakEventListener<DataGridDynamicColumnsBehavior, object, NotifyCollectionChangedEventArgs>(this);
        _weakEventListener.OnEventAction = (instance, source, eventArgs) => instance.OnColumnsCollectionChanged(source, eventArgs);
        _weakEventListener.OnDetachAction = (weakEventListener) => newColumns.CollectionChanged -= weakEventListener.OnEvent;
        newColumns.CollectionChanged += _weakEventListener.OnEvent;
      }

      UpdateColumns();
    }

    private void OnColumnsCollectionChanged(object source, NotifyCollectionChangedEventArgs eventArgs)
    {
      var grid = AssociatedObject;
      if (grid == null)
        return;

      var dataGridColumns = grid.Columns;
      switch (eventArgs.Action)
      {
        case NotifyCollectionChangedAction.Add:
          foreach (var column in eventArgs.NewItems.OfType<DataGridColumn>())
            dataGridColumns.Add(column);
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach (var column in eventArgs.OldItems.OfType<DataGridColumn>())
            dataGridColumns.Remove(column);
          break;
        case NotifyCollectionChangedAction.Replace:
          UpdateColumns();
          break;
        case NotifyCollectionChangedAction.Reset:
          Reset(dataGridColumns);
          break;
      }
    }

    private void UpdateColumns()
    {
      var grid = AssociatedObject;
      if (grid == null)
        return;

      var dataGridColumns = grid.Columns;

      // remove
      Reset(dataGridColumns);

      // add
      if (Columns != null)
        foreach (var column in Columns)
          dataGridColumns.Add(column);

      grid.UpdateLayout();
    }

    private void Reset(IList<DataGridColumn> dataGridColumns)
    {
      int count = dataGridColumns.Count;
      for (int i = count - 1; i >= StartIndex; i--)
        dataGridColumns.RemoveAt(i);
    }
  }
}