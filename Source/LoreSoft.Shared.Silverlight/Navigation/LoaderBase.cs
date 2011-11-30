using System;
using System.Windows;

namespace LoreSoft.Shared.Navigation
{
  public abstract class LoaderBase : DependencyObject
  {
    private bool _cancelled;

    internal ContentLoaderBase ContentLoader { get; set; }

    internal ContentLoaderAsyncResult Result { get; set; }

    public abstract void Cancel();

    public abstract void Load(Uri targetUri, Uri currentUri);

    protected void Complete(Func<object> pageCreator)
    {
      if (_cancelled)
        return;

      Deployment.Current.Dispatcher.BeginInvoke(() => Complete(pageCreator()));
    }

    protected void Complete(Func<Uri> uriCreator)
    {
      if (_cancelled)
        return;

      Deployment.Current.Dispatcher.BeginInvoke(() => Complete(uriCreator()));
    }

    protected void Complete(object page)
    {
      if (_cancelled)
        return;

      Result.Page = page;
      ContentLoader.Complete(Result);
    }

    protected void Complete(Uri redirectUri)
    {
      if (_cancelled)
        return;

      Result.RedirectUri = redirectUri;
      ContentLoader.Complete(Result);
    }

    protected void Error(Exception error)
    {
      Result.Error = error;
      ContentLoader.Complete(Result);
    }

    internal void CancelCore()
    {
      _cancelled = true;
      Cancel();
    }
  }
}