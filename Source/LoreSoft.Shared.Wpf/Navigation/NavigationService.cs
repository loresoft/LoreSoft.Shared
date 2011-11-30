using System;
using System.Windows.Controls;
#if SILVERLIGHT
using LoreSoft.Shared.Collections;
#else
using System.Collections.Concurrent;
#endif

namespace LoreSoft.Shared.Navigation
{
  public class NavigationService : INavigationService
  {
    static NavigationService()
    {
      _targets = new ConcurrentDictionary<string, INavigationService>(StringComparer.OrdinalIgnoreCase);
    }

    private static readonly ConcurrentDictionary<string, INavigationService> _targets;
    public static ConcurrentDictionary<string, INavigationService> Targets
    {
      get { return _targets; }
    }

    private Frame _frame;

    public NavigationService(Frame frame)
    {
      _frame = frame;
    }

    public Uri CurrentSource
    {
      get { return _frame.CurrentSource; }
    }

    public Uri Source
    {
      get { return _frame.Source; }
    }

    public bool CanGoBack
    {
      get { return _frame.CanGoBack; }
    }

    public bool CanGoForward
    {
      get { return _frame.CanGoForward; }
    }

    public bool Navigate(Uri source)
    {
      return _frame.Navigate(source);
    }

    public void GoBack()
    {
      _frame.GoBack();
    }

    public void GoForward()
    {
      _frame.GoForward();
    }

    public void Refresh()
    {
      _frame.Refresh();
    }

    public void StopLoading()
    {
      _frame.StopLoading();
    }

#if !SILVERLIGHT
    public bool Navigate(object source)
    {
      return _frame.Navigate(source);
    }

    public bool Navigate(object source, object navigationState)
    {
      return _frame.Navigate(source, navigationState);
      
    }

    public bool Navigate(Uri source, object navigationState)
    {
      return _frame.Navigate(source, navigationState);      
    }
#endif

  }
}