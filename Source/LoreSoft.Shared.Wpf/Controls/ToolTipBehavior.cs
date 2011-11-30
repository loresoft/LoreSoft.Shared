using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace LoreSoft.Shared.Controls
{
  public class ToolTipBehavior : Behavior<FrameworkElement>
  {
    private bool _isMouseOver;

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.MouseEnter += OnMouseEnter;
      AssociatedObject.MouseLeave += OnMouseLeave;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.MouseEnter -= OnMouseEnter;
      AssociatedObject.MouseLeave -= OnMouseLeave;
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
      var tooltip = ToolTipService.GetToolTip(AssociatedObject) as ToolTip;
      if (tooltip == null)
        return;

      _isMouseOver = false; 
      tooltip.IsOpen = false;
      tooltip.Closed -= OnToolTipClosed;
    }

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
      var tooltip = ToolTipService.GetToolTip(AssociatedObject) as ToolTip;
      if (tooltip == null)
        return;

      _isMouseOver = true;
      tooltip.IsOpen = true;
      tooltip.Closed += OnToolTipClosed;
    }

    private void OnToolTipClosed(object sender, RoutedEventArgs e)
    {
      if (!_isMouseOver)
        return;

      //reopen
      var tooltip = ToolTipService.GetToolTip(AssociatedObject) as ToolTip;
      if (tooltip == null)
        return;

      tooltip.IsOpen = true;
    }

  }
}
