using System;
using System.Windows;
using System.Windows.Media;

namespace LoreSoft.Shared.DragDrop
{
  public class DropTargetHighlightAdorner : DropTargetAdorner
  {
    public DropTargetHighlightAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (DropInfo.VisualTargetItem == null)
        return;

      var location = DropInfo.VisualTargetItem.TranslatePoint(new Point(), AdornedElement);
      var size = VisualTreeHelper.GetDescendantBounds(DropInfo.VisualTargetItem).Size;

      var rect = new Rect(location, size);
      drawingContext.DrawRoundedRectangle(null, new Pen(Brushes.Gray, 2), rect, 2, 2);
    }
  }
}
