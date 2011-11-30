using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LoreSoft.Shared.Controls
{
  public class LabelBehaviour : Behavior<Control>
  {
    private bool _isMouseLeftButtonDown;
    
    #region For
    public FrameworkElement ForElement
    {
      get { return (FrameworkElement)GetValue(ForElementProperty); }
      set { SetValue(ForElementProperty, value); }
    }

    public static readonly DependencyProperty ForElementProperty =
        DependencyProperty.Register(
            "ForElement",
            typeof(FrameworkElement),
            typeof(LabelBehaviour),
            new PropertyMetadata(null));
    #endregion
    
    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
      AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
      AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (e.Handled)
        return;
      
      _isMouseLeftButtonDown = false;
      e.Handled = true;      
      OnClick();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (e.Handled)
        return;
      
      _isMouseLeftButtonDown = true;
      e.Handled = true;
    }

    private void OnClick()
    {
      var target = ForElement as Control;
      if (target == null || !target.IsEnabled)
        return;

      target.Focus();

      if (!(target is ToggleButton))
        return;

      var b = (ToggleButton)target;
      b.IsChecked = true;
    }

  }
}
