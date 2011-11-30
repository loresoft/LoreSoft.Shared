using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace LoreSoft.Shared.DragDrop
{
  public class DropTargetInsertionAdorner : DropTargetAdorner
  {
    private static readonly Pen _pen;
    private static readonly PathGeometry _triangle;

    static DropTargetInsertionAdorner()
    {
      // Create the pen and triangle in a static constructor and freeze them to improve performance.
      const int triangleSize = 3;

      _pen = new Pen(Brushes.Gray, 2);
      _pen.Freeze();

      var firstLine = new LineSegment(new Point(0, -triangleSize), false);
      firstLine.Freeze();
      var secondLine = new LineSegment(new Point(0, triangleSize), false);
      secondLine.Freeze();

      var figure = new PathFigure { StartPoint = new Point(triangleSize, 0) };
      figure.Segments.Add(firstLine);
      figure.Segments.Add(secondLine);
      figure.Freeze();

      _triangle = new PathGeometry();
      _triangle.Figures.Add(figure);
      _triangle.Freeze();
    }

    public DropTargetInsertionAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var itemsControl = DropInfo.VisualTarget as ItemsControl;

      if (itemsControl == null)
        return;

      // Get the position of the item at the insertion index. If the insertion point is
      // to be after the last item, then get the position of the last item and add an 
      // offset later to draw it at the end of the list.
      ItemsControl itemParent = DropInfo.VisualTargetItem != null
                                  ? ItemsControl.ItemsControlFromItemContainer(DropInfo.VisualTargetItem)
                                  : itemsControl;

      int index = Math.Min(DropInfo.InsertIndex, itemParent.Items.Count - 1);
      var itemContainer = (UIElement)itemParent.ItemContainerGenerator.ContainerFromIndex(index);

      if (itemContainer == null)
        return;

      var location = itemContainer.TranslatePoint(new Point(), AdornedElement);
      var renderSize = itemContainer.RenderSize;

      var itemRect = new Rect(location,renderSize);
      Point point1, point2;
      double rotation = 0;

      if (DropInfo.VisualTargetOrientation == Orientation.Vertical)
      {
        if (DropInfo.InsertIndex == itemParent.Items.Count)
          itemRect.Y += renderSize.Height;

        point1 = new Point(itemRect.X, itemRect.Y);
        point2 = new Point(itemRect.Right, itemRect.Y);
      }
      else
      {
        if (DropInfo.InsertIndex == itemParent.Items.Count)
          itemRect.X += renderSize.Width;

        point1 = new Point(itemRect.X, itemRect.Y);
        point2 = new Point(itemRect.X, itemRect.Bottom);
        rotation = 90;
      }

      drawingContext.DrawLine(_pen, point1, point2);
      DrawTriangle(drawingContext, point1, rotation);
      DrawTriangle(drawingContext, point2, 180 + rotation);
    }

    private void DrawTriangle(DrawingContext drawingContext, Point origin, double rotation)
    {
      drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
      drawingContext.PushTransform(new RotateTransform(rotation));

      drawingContext.DrawGeometry(_pen.Brush, null, _triangle);

      drawingContext.Pop();
      drawingContext.Pop();
    }

  }
}
