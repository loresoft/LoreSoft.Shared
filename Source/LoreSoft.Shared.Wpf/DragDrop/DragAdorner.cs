using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace LoreSoft.Shared.DragDrop
{
  class DragAdorner : Adorner
  {
    private readonly AdornerLayer _adornerLayer;
    private readonly UIElement _adornment;
    private Point _mousePosition;

    public DragAdorner(UIElement adornedElement, UIElement adornment)
      : base(adornedElement)
    {
      _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
      _adornerLayer.Add(this);
      _adornment = adornment;
      IsHitTestVisible = false;
    }

    public Point MousePosition
    {
      get { return _mousePosition; }
      set
      {
        if (_mousePosition == value)
          return;

        _mousePosition = value;
        _adornerLayer.Update(AdornedElement);
      }
    }

    public void Detatch()
    {
      _adornerLayer.Remove(this);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      _adornment.Arrange(new Rect(finalSize));
      return finalSize;
    }

    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
      var baseTransform = base.GetDesiredTransform(transform);
      var translateTransform = new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4);
      
      var result = new GeneralTransformGroup();
      if (baseTransform != null)
        result.Children.Add(baseTransform);
      result.Children.Add(translateTransform);

      return result;
    }

    protected override Visual GetVisualChild(int index)
    {
      return _adornment;
    }

    protected override Size MeasureOverride(Size constraint)
    {
      _adornment.Measure(constraint);
      return _adornment.DesiredSize;
    }

    protected override int VisualChildrenCount
    {
      get { return 1; }
    }
  }
}
