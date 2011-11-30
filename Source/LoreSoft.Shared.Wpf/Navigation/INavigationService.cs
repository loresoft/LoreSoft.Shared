using System;

namespace LoreSoft.Shared.Navigation
{
  public interface INavigationService
  {
    Uri CurrentSource { get; }
    Uri Source { get; }
    bool CanGoBack { get; }
    bool CanGoForward { get; }

    bool Navigate(Uri source);
    void GoBack();
    void GoForward();
    void Refresh();
    void StopLoading();

#if !SILVERLIGHT
    bool Navigate(object root);
    bool Navigate(object root, object navigationState);
    bool Navigate(Uri source, Object navigationState);
#endif
  }
}
