using System;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Controls
{
  public class DisposeDataContextBehaviour : Behavior<FrameworkElement>
  {
    #region Recursive
    public bool Recursive
    {
      get { return (bool)GetValue(RecursiveProperty); }
      set { SetValue(RecursiveProperty, value); }
    }

    public static readonly DependencyProperty RecursiveProperty =
        DependencyProperty.Register(
            "Recursive",
            typeof(bool),
            typeof(DisposeDataContextBehaviour),
            new PropertyMetadata(false));
    #endregion

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      AssociatedObject.Unloaded -= OnUnloaded;
      AssociatedObject.Dispatcher.BeginInvoke(new Action(Dispose));
    }

    private void Dispose()
    {
      var disposable = AssociatedObject.DataContext as IDisposable;
      if (disposable != null)
        disposable.Dispose();

      if (!Recursive)
        return;

      AssociatedObject
        .GetVisualTree<FrameworkElement>()
        .Where(d => d.DataContext is IDisposable)
        .Select(d => d.DataContext)
        .Distinct()
        .Cast<IDisposable>()
        .ForEach(d => d.Dispose());
    }
  }
}
