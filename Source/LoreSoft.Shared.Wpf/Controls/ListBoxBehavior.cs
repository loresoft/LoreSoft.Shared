using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace LoreSoft.Shared.Controls
{
  public class ListBoxBehavior : Behavior<ListBox>
  {
    protected override void OnAttached()
    {
      AssociatedObject.Loaded += AssociatedObject_Loaded;
      AssociatedObject.GotFocus += AssociatedObject_GotFocus;
    }

    private void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e)
    {
    }

    private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      if (AssociatedObject.SelectedItem == null)
        return;

      AssociatedObject.UpdateLayout();
      AssociatedObject.ScrollIntoView(AssociatedObject.SelectedItem);
    }
  }
}
