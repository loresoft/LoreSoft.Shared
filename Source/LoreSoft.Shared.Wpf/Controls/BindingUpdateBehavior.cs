using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace LoreSoft.Shared.Controls
{
  public class BindingUpdateBehavior : Behavior<Control>
  {
    private Lazy<Timer> _delayTimer;

    #region Delay
    public static readonly DependencyProperty DelayProperty =
      DependencyProperty.Register(
        "Delay",
        typeof(int),
        typeof(BindingUpdateBehavior),
        new PropertyMetadata(1000));

    public int Delay
    {
      get { return (int)GetValue(DelayProperty); }
      set { SetValue(DelayProperty, value); }
    }

    public static void SetDelay(DependencyObject dependencyObject, int value)
    {
      dependencyObject.SetValue(DelayProperty, value);
    }
    public static int GetDelay(DependencyObject dependencyObject)
    {
      return (int)dependencyObject.GetValue(DelayProperty);
    }
    #endregion Delay

    protected override void OnAttached()
    {
      base.OnAttached();

      _delayTimer = new Lazy<Timer>(() => new Timer(UpdateSource));
      AttachEvent();
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      DetachEvent();

      if (_delayTimer.IsValueCreated)
        _delayTimer.Value.Dispose();

      _delayTimer = null;
    }

    private void AttachEvent()
    {
      if (AssociatedObject is TextBox)
      {
        var textBox = (TextBox)AssociatedObject;
        textBox.TextChanged += OnChanged;
      }
      else if (AssociatedObject is PasswordBox)
      {
        var passwordBox = (PasswordBox)AssociatedObject;
        passwordBox.PasswordChanged += OnChanged;
      }
      else if (AssociatedObject is ToggleButton)
      {
        var toggleButton = (ToggleButton)AssociatedObject;
        toggleButton.Checked += OnChanged;
        toggleButton.Unchecked += OnChanged;
      }
      else if (AssociatedObject is Selector)
      {
        var selector = (Selector)AssociatedObject;
        selector.SelectionChanged += OnChanged;
      }
    }

    private void DetachEvent()
    {
      if (AssociatedObject is TextBox)
      {
        var textBox = (TextBox)AssociatedObject;
        textBox.TextChanged -= OnChanged;
      }
      else if (AssociatedObject is PasswordBox)
      {
        var passwordBox = (PasswordBox)AssociatedObject;
        passwordBox.PasswordChanged -= OnChanged;
      }
      else if (AssociatedObject is ToggleButton)
      {
        var toggleButton = (ToggleButton)AssociatedObject;
        toggleButton.Checked -= OnChanged;
        toggleButton.Unchecked -= OnChanged;
      }
      else if (AssociatedObject is Selector)
      {
        var selector = (Selector)AssociatedObject;
        selector.SelectionChanged -= OnChanged;
      }
    }

    private void OnChanged(object sender, EventArgs e)
    {
      // keep updating timer on change
      if (AssociatedObject is TextBox || AssociatedObject is PasswordBox)
        _delayTimer.Value.Change(Delay, Timeout.Infinite);
      else
        UpdateSource();
    }

    private void UpdateSource()
    {
      if (AssociatedObject is TextBox)
      {
        var expression = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
        if (expression != null)
          expression.UpdateSource();
      }
#if SILVERLIGHT
      else if (AssociatedObject is PasswordBox)
      {
        var expression = AssociatedObject.GetBindingExpression(PasswordBox.PasswordProperty);
        if (expression != null)
          expression.UpdateSource();
      }
#endif
      else if (AssociatedObject is ToggleButton)
      {
        var expression = AssociatedObject.GetBindingExpression(ToggleButton.IsCheckedProperty);
        if (expression != null)
          expression.UpdateSource();
      }
      else if (AssociatedObject is Selector)
      {
        var expression = AssociatedObject.GetBindingExpression(Selector.SelectedValueProperty);
        if (expression != null)
          expression.UpdateSource();

        expression = AssociatedObject.GetBindingExpression(Selector.SelectedItemProperty);
        if (expression != null)
          expression.UpdateSource();
      }
    }

    // called from timer background thread
    private void UpdateSource(object state)
    {
      AssociatedObject.Dispatcher.BeginInvoke(new Action(UpdateSource));
    }
  }
}