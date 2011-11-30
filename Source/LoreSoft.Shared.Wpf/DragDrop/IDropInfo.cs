using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LoreSoft.Shared.DragDrop
{
  public interface IDropInfo
  {
    /// <summary>
    /// Gets the drag data.
    /// </summary>
    /// <remarks>
    /// If the drag came from within the framework, this will hold:
    /// 
    /// - The dragged data if a single item was dragged.
    /// - A typed IEnumerable if multiple items were dragged.
    /// </remarks>
    object Data { get; }

    /// <summary>
    /// Gets a <see cref="DragInfo"/> object holding information about the source of the drag, 
    /// if the drag came from within the framework.
    /// </summary>
    IDragInfo DragInfo { get; }

    /// <summary>
    /// Gets the mouse position relative to the VisualTarget
    /// </summary>
    Point DropPosition { get; }

    /// <summary>
    /// Gets or sets the class of drop target to display.
    /// </summary>
    /// <remarks>
    /// The standard drop target adorner classes are held in the <see cref="DropTargetAdorners"/> class.
    /// </remarks>
    Type DropTargetAdorner { get; set; }

    /// <summary>
    /// Gets or sets the allowed effects for the drop.
    /// </summary>
    /// <remarks>
    /// This must be set to a value other than <see cref="DragDropEffects.None"/> by a drop handler
    /// in order for a drop to be possible.
    /// </remarks>
    DragDropEffects Effects { get; set; }

    /// <summary>
    /// Gets the current insert position within <see cref="TargetCollection"/>.
    /// </summary>
    int InsertIndex { get; }

    /// <summary>
    /// Gets the collection that the target ItemsControl is bound to.
    /// </summary>
    /// 
    /// <remarks>
    /// If the current drop target is unbound or not an ItemsControl, this will be null.
    /// </remarks>
    IEnumerable TargetCollection { get; }

    /// <summary>
    /// Gets the object that the current drop target is bound to.
    /// </summary>
    /// <remarks>
    /// If the current drop target is unbound or not an ItemsControl, this will be null.
    /// </remarks>
    object TargetItem { get; }

    /// <summary>
    /// Gets the current group target.
    /// </summary>
    /// <remarks>
    /// If the drag is currently over an ItemsControl with groups, describes the group that
    /// the drag is currently over.
    /// </remarks>
    CollectionViewGroup TargetGroup { get; }

    /// <summary>
    /// Gets the control that is the current drop target.
    /// </summary>
    UIElement VisualTarget { get; }

    /// <summary>
    /// Gets the item in an ItemsControl that is the current drop target.
    /// </summary>
    /// <remarks>
    /// If the current drop target is unbound or not an ItemsControl, this will be null.
    /// </remarks>
    UIElement VisualTargetItem { get; }

    /// <summary>
    /// Gets th orientation of the current drop target.
    /// </summary>
    Orientation VisualTargetOrientation { get; }
  }
}
