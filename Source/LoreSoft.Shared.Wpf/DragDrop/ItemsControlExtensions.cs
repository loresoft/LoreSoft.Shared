using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using LoreSoft.Shared.Extensions;
using LoreSoft.Shared.Reflection;

namespace LoreSoft.Shared.DragDrop
{
  public static class ItemsControlExtensions
  {
    public static bool CanSelectMultipleItems(this ItemsControl itemsControl)
    {
      if (itemsControl is MultiSelector)
        return (bool)LateBinder.GetProperty(itemsControl,
          "CanSelectMultipleItems",
          BindingFlags.Instance | BindingFlags.NonPublic);

      if (itemsControl is ListBox)
        return ((ListBox)itemsControl).SelectionMode != SelectionMode.Single;

      return false;
    }

    public static UIElement GetItemContainer(this ItemsControl itemsControl, UIElement child)
    {
      Type itemType = GetItemContainerType(itemsControl);
      return itemType == null ? null : (UIElement)child.GetVisualAncestor(itemType);
    }

    public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position)
    {
      var inputElement = itemsControl.InputHitTest(position);
      var uiElement = inputElement as UIElement;
      return uiElement == null ? null : GetItemContainer(itemsControl, uiElement);
    }

    public static UIElement GetItemContainerAt(this ItemsControl itemsControl, Point position, Orientation searchDirection)
    {
      Type itemContainerType = GetItemContainerType(itemsControl);

      if (itemContainerType == null)
        return null;

      LineGeometry line;

      switch (searchDirection)
      {
        case Orientation.Horizontal:
          line = new LineGeometry(new Point(0, position.Y), new Point(itemsControl.RenderSize.Width, position.Y));
          break;
        case Orientation.Vertical:
          line = new LineGeometry(new Point(position.X, 0), new Point(position.X, itemsControl.RenderSize.Height));
          break;
        default:
          throw new ArgumentException("Invalid value for searchDirection");
      }

      var hits = new List<DependencyObject>();

      VisualTreeHelper.HitTest(
        itemsControl,
        null,
        result =>
        {
          DependencyObject itemContainer = result.VisualHit.GetVisualAncestor(itemContainerType);
          if (itemContainer != null)
            hits.Add(itemContainer);
          return HitTestResultBehavior.Continue;
        },
        new GeometryHitTestParameters(line));

      return GetClosest(itemsControl, hits, position, searchDirection);
    }

    public static Type GetItemContainerType(this ItemsControl itemsControl)
    {
      if (itemsControl is ListView)
        return typeof(ListViewItem);
      if (itemsControl is TreeView)
        return typeof(TreeViewItem);
      if (itemsControl is ListBox)
        return typeof(ListBoxItem);

      if (itemsControl.Items.Count <= 0)
        return null;

      IEnumerable<ItemsPresenter> itemsPresenters = itemsControl.GetVisualDescendants().OfType<ItemsPresenter>();

      return (from itemsPresenter in itemsPresenters
              select VisualTreeHelper.GetChild(itemsPresenter, 0)
                into panel
                select VisualTreeHelper.GetChild(panel, 0)
                  into itemContainer
                  where itemContainer != null && itemsControl.ItemContainerGenerator.IndexFromContainer(itemContainer) != -1
                  select itemContainer.GetType()).FirstOrDefault();
    }

    public static Orientation GetItemsPanelOrientation(this ItemsControl itemsControl)
    {
      var itemsPresenter = itemsControl.GetVisualDescendant<ItemsPresenter>();
      if (itemsPresenter == null)
        return Orientation.Vertical;

      var itemsPanel = VisualTreeHelper.GetChild(itemsPresenter, 0);
      if (itemsPanel == null)
        return Orientation.Vertical;

      var orientationProperty = LateBinder.FindProperty(itemsPanel.GetType(), "Orientation");
      if (orientationProperty == null)
        return Orientation.Vertical;

      return (Orientation)orientationProperty.GetValue(itemsPanel);
    }

    public static void SetSelectedItem(this ItemsControl itemsControl, object item)
    {
      if (itemsControl is MultiSelector)
      {
        ((MultiSelector)itemsControl).SelectedItem = null;
        ((MultiSelector)itemsControl).SelectedItem = item;
      }
      else if (itemsControl is ListBox)
      {
        ((ListBox)itemsControl).SelectedItem = null;
        ((ListBox)itemsControl).SelectedItem = item;
      }
      else if (itemsControl is TreeView)
      {
        // TODO: Select the TreeViewItem
        //((TreeView)itemsControl)
      }
      else if (itemsControl is Selector)
      {
        ((Selector)itemsControl).SelectedItem = null;
        ((Selector)itemsControl).SelectedItem = item;
      }
    }

    public static IEnumerable GetSelectedItems(this ItemsControl itemsControl)
    {
      if (itemsControl is MultiSelector)
        return ((MultiSelector)itemsControl).SelectedItems;
      
      if (itemsControl is ListBox)
      {
        var listBox = (ListBox)itemsControl;
        return listBox.SelectionMode == SelectionMode.Single 
          ? (IEnumerable)Enumerable.Repeat(listBox.SelectedItem, 1) 
          : listBox.SelectedItems;
      }

      if (itemsControl is TreeView)
        return Enumerable.Repeat(((TreeView)itemsControl).SelectedItem, 1);
      
      if (itemsControl is Selector)
        return Enumerable.Repeat(((Selector)itemsControl).SelectedItem, 1);

      return Enumerable.Empty<object>();
    }

    public static bool GetItemSelected(this ItemsControl itemsControl, object item)
    {
      if (itemsControl is MultiSelector)
        return ((MultiSelector)itemsControl).SelectedItems.Contains(item);
      
      if (itemsControl is ListBox)
        return ((ListBox)itemsControl).SelectedItems.Contains(item);
      
      if (itemsControl is TreeView)
        return ((TreeView)itemsControl).SelectedItem == item;
      
      if (itemsControl is Selector)
        return ((Selector)itemsControl).SelectedItem == item;
      
      return false;
    }

    public static void SetItemSelected(this ItemsControl itemsControl, object item, bool value)
    {
      if (itemsControl is MultiSelector)
      {
        var multiSelector = (MultiSelector)itemsControl;

        if (value)
        {
          if (multiSelector.CanSelectMultipleItems())
            multiSelector.SelectedItems.Add(item);
          else
            multiSelector.SelectedItem = item;
        }
        else
        {
          multiSelector.SelectedItems.Remove(item);
        }
      }
      else if (itemsControl is ListBox)
      {
        var listBox = (ListBox)itemsControl;

        if (value)
        {
          if (listBox.SelectionMode != SelectionMode.Single)
            listBox.SelectedItems.Add(item);
          else
            listBox.SelectedItem = item;
        }
        else
        {
          listBox.SelectedItems.Remove(item);
        }
      }
    }

    private static UIElement GetClosest(ItemsControl itemsControl, IEnumerable<DependencyObject> items, Point position, Orientation searchDirection)
    {
      UIElement closest = null;
      double closestDistance = double.MaxValue;

      foreach (DependencyObject i in items)
      {
        var uiElement = i as UIElement;

        if (uiElement == null)
          continue;

        Point p = uiElement.TransformToAncestor(itemsControl).Transform(new Point(0, 0));
        double distance = double.MaxValue;

        switch (searchDirection)
        {
          case Orientation.Horizontal:
            distance = Math.Abs(position.X - p.X);
            break;
          case Orientation.Vertical:
            distance = Math.Abs(position.Y - p.Y);
            break;
        }

        if (distance >= closestDistance)
          continue;

        closest = uiElement;
        closestDistance = distance;
      }

      return closest;
    }
  }
}
