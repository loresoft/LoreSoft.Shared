using System;
using System.Windows;
using System.Windows.Navigation;

namespace LoreSoft.Shared.Navigation
{
  public abstract class ContentLoaderBase : DependencyObject, INavigationContentLoader
  {
    #region INavigationContentLoader Members

    public IAsyncResult BeginLoad(Uri targetUri, Uri currentUri, AsyncCallback userCallback, object asyncState)
    {
      LoaderBase loader = CreateLoaderCore();
      var result = new ContentLoaderAsyncResult(asyncState, loader, userCallback);
      result.BeginLoadCompleted = false;
      loader.Result = result;

      lock (result.Lock)
      {
        loader.Load(targetUri, currentUri);
        result.BeginLoadCompleted = true;
        return result;
      }
    }

    public virtual bool CanLoad(Uri targetUri, Uri currentUri)
    {
      return true;
    }

    public void CancelLoad(IAsyncResult asyncResult)
    {
      var loaderAsyncResult = (ContentLoaderAsyncResult)asyncResult;
      loaderAsyncResult.Loader.CancelCore();
    }

    public LoadResult EndLoad(IAsyncResult asyncResult)
    {
      var loaderAsyncResult = (ContentLoaderAsyncResult)asyncResult;
      if (loaderAsyncResult.Error != null)
        throw loaderAsyncResult.Error;

      return loaderAsyncResult.Page != null
               ? new LoadResult(loaderAsyncResult.Page)
               : new LoadResult(loaderAsyncResult.RedirectUri);
    }

    #endregion

    protected abstract LoaderBase CreateLoader();

    private LoaderBase CreateLoaderCore()
    {
      LoaderBase loader = CreateLoader();
      loader.ContentLoader = this;
      return loader;
    }

    internal void Complete(IAsyncResult result)
    {
      var asyncResult = (ContentLoaderAsyncResult)result;

      lock (asyncResult.Lock)
      {
        if (Dispatcher.CheckAccess())
          asyncResult.Callback(result);
        else
          Dispatcher.BeginInvoke(() => asyncResult.Callback(result));
      }
    }
  }
}