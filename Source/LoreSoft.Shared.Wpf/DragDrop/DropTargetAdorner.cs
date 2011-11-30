using System;
using System.Windows.Documents;
using System.Windows;

namespace LoreSoft.Shared.DragDrop
{
  public abstract class DropTargetAdorner : Adorner
  {
    private readonly AdornerLayer _adornerLayer;

    protected DropTargetAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
      _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
      _adornerLayer.Add(this);
      IsHitTestVisible = false;
    }

    public void Detatch()
    {
      _adornerLayer.Remove(this);
    }

    public DropInfo DropInfo { get; set; }

    internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
    {
      if (!typeof (DropTargetAdorner).IsAssignableFrom(type))
        throw new InvalidOperationException(
          "The requested adorner class does not derive from DropTargetAdorner.");

      return (DropTargetAdorner)type.GetConstructor(new[] { typeof(UIElement) })
          .Invoke(new[] { adornedElement });
    }

  }
}
