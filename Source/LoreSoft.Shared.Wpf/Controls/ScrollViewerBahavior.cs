using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Controls
{
  public class ScrollViewerBahavior : Behavior<ScrollViewer>
  {
    private const int SCROLL_PADDING = 10;

    protected override void OnAttached()
    {
      base.OnAttached();

      this.AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();

      this.AssociatedObject.Loaded -= OnLoaded;
      var controls = this.AssociatedObject.GetVisualDescendants();
      foreach (FrameworkElement control in controls)
        control.GotFocus -= OnControlGotFocus;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      var controls = AssociatedObject.GetVisualDescendants();
      foreach (FrameworkElement control in controls)
        control.GotFocus += OnControlGotFocus;
    }

    private void OnControlGotFocus(object sender, RoutedEventArgs e)
    {
      var element = e.OriginalSource as FrameworkElement;
      if (element == null)
        return;

      var transform = element.TransformToVisual(AssociatedObject);
      var positionInScrollViewer = transform.Transform(new Point(0, 0));

      if (positionInScrollViewer.Y < 0 || positionInScrollViewer.Y > AssociatedObject.ViewportHeight)
        AssociatedObject.ScrollToVerticalOffset(AssociatedObject.VerticalOffset + positionInScrollViewer.Y - SCROLL_PADDING);

      if (positionInScrollViewer.X < 0 || positionInScrollViewer.X > AssociatedObject.ViewportWidth)
        AssociatedObject.ScrollToHorizontalOffset(AssociatedObject.HorizontalOffset + positionInScrollViewer.X - SCROLL_PADDING);
    }

  };
}
