using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LoreSoft.Shared.Controls
{
  public class DataGridColumnBehavior : Behavior<DataGridColumn>
  {

    #region Header
    public object Header
    {
      get { return (object)GetValue(HeaderProperty); }
      set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(DataGridColumnBehavior),
            new PropertyMetadata(null, OnHeaderChanged));

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as DataGridColumnBehavior;
      if (source == null)
        return;

      source.OnHeaderChanged(e);
    }

    protected virtual void OnHeaderChanged(DependencyPropertyChangedEventArgs e)
    {
      UpdateHeader();
    }
    #endregion

    #region Visibility
    public Visibility Visibility
    {
      get { return (Visibility)GetValue(VisibilityProperty); }
      set { SetValue(VisibilityProperty, value); }
    }

    public static readonly DependencyProperty VisibilityProperty =
        DependencyProperty.Register(
            "Visibility",
            typeof(Visibility),
            typeof(DataGridColumnBehavior),
            new PropertyMetadata(Visibility.Visible, OnVisibilityChanged));

    private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as DataGridColumnBehavior;
      if (source == null)
        return;

      source.OnVisibilityChanged(e);
    }

    protected virtual void OnVisibilityChanged(DependencyPropertyChangedEventArgs e)
    {
      UpdateVisibility();
    }
    #endregion

    protected override void OnAttached()
    {
      if (Header != null)
        UpdateHeader();

      UpdateVisibility();
    }

    private void UpdateHeader()
    {
      if (AssociatedObject == null)
        return;

      AssociatedObject.Header = Header;
    }

    private void UpdateVisibility()
    {
      if (AssociatedObject == null)
        return;

      AssociatedObject.Visibility = Visibility;
    }

  }
}
