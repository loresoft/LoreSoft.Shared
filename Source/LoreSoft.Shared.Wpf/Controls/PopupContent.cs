using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LoreSoft.Shared.Controls
{
  [TemplatePart(Name = ElementPopupName, Type = typeof(Popup))]
  public class PopupContent : ContentControl
  {
    private const string ElementPopupName = "PopupElement";

    private Popup ElementPopup;
    private FrameworkElement ElementPopupChild;

    private bool _isMouseOver;


    public PopupContent()
    {
      this.DefaultStyleKey = typeof(PopupContent);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      ElementPopup = GetTemplateChild(ElementPopupName) as Popup;

      ElementPopupChild = ElementPopup != null
        ? ElementPopup.Child as FrameworkElement
        : null;

      SizeChanged += ElementPopupChild_SizeChanged;
      if (ElementPopupChild != null)
      {
        ElementPopupChild.MouseEnter += ElementPopupChild_MouseEnter;
        ElementPopupChild.MouseLeave += ElementPopupChild_MouseLeave;
        ElementPopupChild.SizeChanged += ElementPopupChild_SizeChanged;
      }

    }

    protected override void OnMouseEnter(MouseEventArgs e)
    {
      base.OnMouseEnter(e);
      _isMouseOver = true;
      ShowPopup();
    }
    
    protected override void OnMouseLeave(MouseEventArgs e)
    {
      base.OnMouseLeave(e);
      _isMouseOver = false;
      HidePopup();
    }
    
    private void ElementPopupChild_MouseLeave(object sender, MouseEventArgs e)
    {
      _isMouseOver = false;
      HidePopup();
    }

    private void ElementPopupChild_MouseEnter(object sender, MouseEventArgs e)
    {
      _isMouseOver = true;
      ShowPopup();
    }

    private void ElementPopupChild_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ArrangePopup();
    }

    private void ArrangePopup()
    {
      if (ElementPopup == null || ElementPopupChild == null)
        return;

      ElementPopup.HorizontalOffset = HorizontalOffset;
      ElementPopup.VerticalOffset = VerticalOffset;
    }

    private void ShowPopup()
    {
      if (ElementPopup == null)
        return;

      ElementPopup.IsOpen = true;
    }
    
    private void HidePopup()
    {
      if (ElementPopup == null)
        return;

      ElementPopup.IsOpen = false;
    }

    #region Popup
    public object Popup
    {
      get { return (object)GetValue(PopupProperty); }
      set { SetValue(PopupProperty, value); }
    }

    public static readonly DependencyProperty PopupProperty =
        DependencyProperty.Register(
            "Popup",
            typeof(object),
            typeof(PopupContent),
            new PropertyMetadata(null));
    #endregion
    
    #region PopupTemplate
    public DataTemplate PopupTemplate
    {
      get { return (DataTemplate)GetValue(PopupTemplateProperty); }
      set { SetValue(PopupTemplateProperty, value); }
    }

    public static readonly DependencyProperty PopupTemplateProperty =
        DependencyProperty.Register(
            "PopupTemplate",
            typeof(DataTemplate),
            typeof(PopupContent),
            new PropertyMetadata(null));

    
    #endregion
    
    #region HorizontalOffset
    public double HorizontalOffset
    {
      get { return (double)GetValue(HorizontalOffsetProperty); }
      set { SetValue(HorizontalOffsetProperty, value); }
    }

    public static readonly DependencyProperty HorizontalOffsetProperty =
        DependencyProperty.Register(
            "HorizontalOffset",
            typeof(double),
            typeof(PopupContent),
            new PropertyMetadata(0D, OnHorizontalOffsetChanged));

    private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as PopupContent;
      if (source == null)
        return;

      source.OnHorizontalOffsetChanged(e);
    }

    protected virtual void OnHorizontalOffsetChanged(DependencyPropertyChangedEventArgs e)
    {
      ArrangePopup();
    }
    #endregion
    
    #region VerticalOffset
    public double VerticalOffset
    {
      get { return (double)GetValue(VerticalOffsetProperty); }
      set { SetValue(VerticalOffsetProperty, value); }
    }

    public static readonly DependencyProperty VerticalOffsetProperty =
        DependencyProperty.Register(
            "VerticalOffset",
            typeof(double),
            typeof(PopupContent),
            new PropertyMetadata(0D, OnVerticalOffsetChanged));

    private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var source = d as PopupContent;
      if (source == null)
        return;

      source.OnVerticalOffsetChanged(e);
    }

    protected virtual void OnVerticalOffsetChanged(DependencyPropertyChangedEventArgs e)
    {
      ArrangePopup();
    }
    #endregion
  }
}
