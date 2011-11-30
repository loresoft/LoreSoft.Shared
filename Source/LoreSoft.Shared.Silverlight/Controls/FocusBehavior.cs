using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace LoreSoft.Shared.Controls
{
  public enum FocusState
  {
    Always,
    NullOrEmpty,
    NoOtherFocus
  }
  
  public class FocusBehavior : Behavior<Control>
  {
    #region When
    public static readonly DependencyProperty WhenProperty =
      DependencyProperty.Register(
        "When",
        typeof(FocusState),
        typeof(FocusBehavior),
        new PropertyMetadata(FocusState.Always));

    public FocusState When
    {
      get { return (FocusState)GetValue(WhenProperty); }
      set { SetValue(WhenProperty, value); }
    }

    public static void SetWhen(DependencyObject dependencyObject, FocusState value)
    {
      dependencyObject.SetValue(WhenProperty, value);
    }
    public static FocusState GetWhen(DependencyObject dependencyObject)
    {
      return (FocusState)dependencyObject.GetValue(WhenProperty);
    }
    #endregion When

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      AssociatedObject.Loaded -= OnLoaded;

      if (!ShouldFocus())
        return;

      if (!Application.Current.IsRunningOutOfBrowser)
        HtmlPage.Plugin.Focus();

      AssociatedObject.Focus();
    }

    private bool ShouldFocus()
    {
      switch (When)
      {
        case FocusState.Always:
          return true;
        case FocusState.NullOrEmpty:
          // only TextBox and PasswordBox supported
          if (AssociatedObject is TextBox)
            return string.IsNullOrWhiteSpace(((TextBox)AssociatedObject).Text);
          if (AssociatedObject is PasswordBox)
            return string.IsNullOrWhiteSpace(((PasswordBox)AssociatedObject).Password);

          return true;
        default:
          return FocusManager.GetFocusedElement() == null;
      }
    }
  }
}