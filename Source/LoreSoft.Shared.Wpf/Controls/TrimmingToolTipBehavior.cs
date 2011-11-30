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
  public class TrimmingToolTipBehavior : Behavior<TextBlock>
  {
    private FrameworkElement _parentElement;

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.TextTrimming = TextTrimming.WordEllipsis;      
      AssociatedObject.SizeChanged += OnSizeChanged;
      _parentElement = VisualTreeHelper.GetParent(AssociatedObject) as FrameworkElement;
      if (_parentElement != null)
        _parentElement.SizeChanged += OnSizeChanged;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.SizeChanged -= OnSizeChanged;
      if (_parentElement != null)
        _parentElement.SizeChanged -= OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      ComputeTooltip();
    }
    

    private void ComputeTooltip()
    {
      bool isEnabled = false;

#if SILVERLIGHT
      var parentElement = VisualTreeHelper.GetParent(AssociatedObject) as FrameworkElement;
      isEnabled = parentElement != null && AssociatedObject.ActualWidth > parentElement.ActualWidth;
#else
      AssociatedObject.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
      double desiredWidth = AssociatedObject.DesiredSize.Width;
      isEnabled = AssociatedObject.ActualWidth < desiredWidth;
#endif

      string tooltip = isEnabled ? AssociatedObject.Text : null;
      ToolTipService.SetToolTip(AssociatedObject, tooltip);
    }
  }
}
