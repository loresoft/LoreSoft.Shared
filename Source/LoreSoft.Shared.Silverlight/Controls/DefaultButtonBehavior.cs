using System;
using System.Net;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
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
  public class DefaultButtonBehavior : Behavior<FrameworkElement>
  {

    #region DefaultButton (DependencyProperty)
    public string DefaultButton
    {
      get { return (string)GetValue(DefaultButtonProperty); }
      set { SetValue(DefaultButtonProperty, value); }
    }
    public static readonly DependencyProperty DefaultButtonProperty =
        DependencyProperty.Register("DefaultButton", typeof(string), typeof(DefaultButtonBehavior),
          new PropertyMetadata(null));

    #endregion
    

    protected override void OnAttached()
    {
      AssociatedObject.KeyDown += DefaultButtonBehaviorKeyDown;
    }

    protected override void OnDetaching()
    {
      AssociatedObject.KeyDown -= DefaultButtonBehaviorKeyDown;
    }

    private void DefaultButtonBehaviorKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Enter || string.IsNullOrEmpty(DefaultButton))
        return;

      var view = sender as FrameworkElement;
      if (view == null)
        return;

      var button = view.FindName(DefaultButton) as Button;
      if (button == null || !button.IsEnabled)
        return;

      // focus to trigger data binding update
      button.Focus();

      var peer = new ButtonAutomationPeer(button);
      var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
      if (invokeProvider == null)
        return;

      // run async to give bind a chance to update
      AssociatedObject.Dispatcher.BeginInvoke(invokeProvider.Invoke);
    }
  }
}
