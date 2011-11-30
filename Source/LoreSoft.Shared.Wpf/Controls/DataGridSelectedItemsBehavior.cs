using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace LoreSoft.Shared.Controls
{
  public class DataGridSelectedItemsBehavior : Behavior<DataGrid>
  {
    #region SelectedItems
    public IList SelectedItems
    {
      get { return (IList)GetValue(SelectedItemsProperty); }
      set { SetValue(SelectedItemsProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register(
            "SelectedItems",
            typeof(ICollection<Object>),
            typeof(DataGridSelectedItemsBehavior),
            new PropertyMetadata(null, OnSelectedItemsChanged));

    private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as DataGridSelectedItemsBehavior;
      if (source == null)
        return;

      source.OnSelectedItemsChanged(e);
    }

    protected virtual void OnSelectedItemsChanged(DependencyPropertyChangedEventArgs e)
    {

    }
    #endregion

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.SelectionChanged += OnSelectionChanged;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.SelectionChanged -= OnSelectionChanged;
      base.OnDetaching();
    }    
    
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      SelectedItems = AssociatedObject.SelectedItems;
    }

  }
}
