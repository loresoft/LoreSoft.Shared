using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///   An INavigationContentLoader that loads a different page when an error is encountered in the loading process.
  ///   This INavigationContentLoader allows you to provide error pages akin to a 404 page on the web.
  /// </summary>
  [ContentProperty("ErrorPages")]
  public class ErrorPageLoader : ContentLoaderBase
  {
    /// <summary>
    ///   The INavigationContentLoader being wrapped by the ErrorPageLoader.
    /// </summary>
    public static readonly DependencyProperty ContentLoaderProperty =
      DependencyProperty.Register("ContentLoader",
                                  typeof (INavigationContentLoader),
                                  typeof (ErrorPageLoader),
                                  new PropertyMetadata(new PageResourceContentLoader()));

    /// <summary>
    ///   The INavigationContentLoader to use to load the error pages.  If this is unset, the ErrorPageLoader will use
    ///   the ContentLoader to load the ErrorPage.
    /// </summary>
    public static readonly DependencyProperty ErrorContentLoaderProperty =
      DependencyProperty.Register("ErrorContentLoader",
                                  typeof (INavigationContentLoader),
                                  typeof (ErrorPageLoader),
                                  new PropertyMetadata(null));

    /// <summary>
    ///   The attached "Error" property, set when an ErrorPageLoader loads a page for an error.
    /// </summary>
    public static readonly DependencyProperty ErrorProperty =
      DependencyProperty.RegisterAttached("Error",
                                          typeof (Exception),
                                          typeof (ErrorPageLoader),
                                          new PropertyMetadata(null));


    /// <summary>
    ///   Constructs an ErrorPageLoader.
    /// </summary>
    public ErrorPageLoader()
    {
      ErrorPages = new DependencyObjectCollection<IErrorPage>();
    }


    /// <summary>
    ///   The INavigationContentLoader being wrapped by the ErrorPageLoader.
    /// </summary>
    public INavigationContentLoader ContentLoader
    {
      get { return (INavigationContentLoader)GetValue(ContentLoaderProperty); }
      set { SetValue(ContentLoaderProperty, value); }
    }

    /// <summary>
    ///   The INavigationContentLoader to use to load the error pages.  If this is unset, the ErrorPageLoader will use
    ///   the ContentLoader to load the ErrorPage.
    /// </summary>
    [TypeConverter(typeof (ErrorContentLoaderConverter))]
    public INavigationContentLoader ErrorContentLoader
    {
      get { return (INavigationContentLoader)GetValue(ErrorContentLoaderProperty); }
      set { SetValue(ErrorContentLoaderProperty, value); }
    }

    /// <summary>
    ///   The set of IErrorPages that will be used to determine what Uri to load when an error is encountered.
    /// </summary>
    public DependencyObjectCollection<IErrorPage> ErrorPages { get; private set; }


    /// <summary>
    ///   Gets the attached "Error" property, set when an ErrorPageLoader loads a page for an error.
    /// </summary>
    /// <param name = "obj">A dependency object that the error is attached to.</param>
    /// <returns>The exception that caused the error page to be shown.</returns>
    public static Exception GetError(DependencyObject obj)
    {
      return (Exception)obj.GetValue(ErrorProperty);
    }

    /// <summary>
    ///   Sets the attached "Error" property, automatically when an ErrorPageLoader loads a page for an error.
    /// </summary>
    /// <param name = "obj">A dependency object that the error is attached to.</param>
    /// <param name = "value">The exception that caused the error page to be shown.</param>
    public static void SetError(DependencyObject obj, Exception value)
    {
      obj.SetValue(ErrorProperty, value);
    }

    /// <summary>
    ///   Creates an instance of a LoaderBase that will be used to handle loading.
    /// </summary>
    /// <returns>An instance of a LoaderBase.</returns>
    protected override LoaderBase CreateLoader()
    {
      return new Loader(this);
    }

    #region Nested type: Loader

    private class Loader : LoaderBase
    {
      private readonly IEnumerable<IErrorPage> _errorPages;
      private readonly ErrorPageLoader _parent;
      private INavigationContentLoader _loader;
      private IAsyncResult _result;

      public Loader(ErrorPageLoader parent)
      {
        _loader = parent.ContentLoader;
        _parent = parent;
        _errorPages = parent.ErrorPages;
      }

      public override void Cancel()
      {
        _loader.CancelLoad(_result);
      }

      public override void Load(Uri targetUri, Uri currentUri)
      {
        _loader = _parent.ContentLoader;
        try
        {
          if (!_loader.CanLoad(targetUri, currentUri))
            throw new CannotLoadException(_loader, targetUri, currentUri);
          _result = _loader.BeginLoad(targetUri, currentUri, res =>
          {
            try
            {
              LoadResult lr = _loader.EndLoad(res);
              if (lr.LoadedContent != null)
              {
                if (!(lr.LoadedContent is UserControl))
                  throw new InvalidContentException(
                    lr.LoadedContent);
                Complete(lr.LoadedContent);
                return;
              }
              Complete(lr.RedirectUri);
              return;
            }
            catch (Exception e)
            {
              var ep = GetErrorPage(e);
              if (ep == null)
              {
                Error(e);
                return;
              }
              ErrorLoad(ep.Map(e), currentUri, e);
            }
          }, null);
        }
        catch (Exception e)
        {
          var ep = GetErrorPage(e);
          if (ep == null)
          {
            Error(e);
            return;
          }
          ErrorLoad(ep.Map(e), currentUri, e);
        }
      }

      private void ErrorLoad(Uri errorTarget, Uri currentUri, Exception ex)
      {
        _loader = _parent.ErrorContentLoader ?? _parent.ContentLoader;
        try
        {
          _result = _loader.BeginLoad(errorTarget, currentUri, res =>
          {
            try
            {
              LoadResult lr = _loader.EndLoad(res);
              if (lr.LoadedContent != null)
              {
                if (lr.LoadedContent is DependencyObject)
                  SetError(
                    (lr.LoadedContent as DependencyObject),
                    ex);
                Complete(lr.LoadedContent);
                return;
              }
              Complete(lr.RedirectUri);
              return;
            }
            catch (Exception e)
            {
              Error(e);
              return;
            }
          }, null);
        }
        catch (Exception e)
        {
          Error(e);
          return;
        }
      }

      private IErrorPage GetErrorPage(Exception ex)
      {
        return (from ep in _errorPages
                where ep.Matches(ex)
                select ep).FirstOrDefault();
      }
    }

    #endregion
  }
}