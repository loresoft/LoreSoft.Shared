using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace LoreSoft.Shared.Navigation
{
  public class NavigationServiceBehavior : Behavior<Frame>
  {

#if !SILVERLIGHT
    #region SourceContent
    public object SourceContent
    {
      get { return GetValue(SourceContentProperty); }
      set { SetValue(SourceContentProperty, value); }
    }

    public static readonly DependencyProperty SourceContentProperty =
        DependencyProperty.Register(
            "SourceContent",
            typeof(object),
            typeof(NavigationServiceBehavior));
    #endregion
#endif

    protected override void OnAttached()
    {
      base.OnAttached();
      AssociatedObject.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      INavigationService service;
      NavigationService.Targets.TryRemove(AssociatedObject.Name,out service);
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      AssociatedObject.Loaded -= OnLoaded;

      var service = new NavigationService(AssociatedObject);
      NavigationService.Targets.TryAdd(AssociatedObject.Name, service);

#if !SILVERLIGHT
      if (SourceContent == null)
        return;

      service.Navigate(SourceContent);
#endif
    }
  }
}