using System;
using System.Security.Principal;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///  An <see cref="INavigationContentLoader"/> that throws an <see cref="UnauthorizedAccessException"/> 
  ///  if the user does not meet the requirements specified for the page they are trying to navigate to.
  /// </summary>
  [ContentProperty("Authorizer")]
  public class AuthorizationContentLoader : ContentLoaderBase
  {
    private static readonly INavigationContentLoader _defaultLoader = new PageResourceContentLoader();

    /// <summary>
    ///   The Authorizer that will be used to authorize the Principal.
    /// </summary>
    public static readonly DependencyProperty AuthorizerProperty =
      DependencyProperty.Register("Authorizer",
                                  typeof(INavigationAuthorizer),
                                  typeof(AuthorizationContentLoader),
                                  new PropertyMetadata(null));

    /// <summary>
    ///   The <see cref="INavigationContentLoader"/> being wrapped by the <see cref="AuthorizationContentLoader"/>.
    /// </summary>
    public static readonly DependencyProperty ContentLoaderProperty =
      DependencyProperty.Register("ContentLoader",
                                  typeof(INavigationContentLoader),
                                  typeof(AuthorizationContentLoader),
                                  new PropertyMetadata(null));

    /// <summary>
    ///   The <see cref="IPrincipal"/> that will be used to check authorization.
    /// </summary>
    public static readonly DependencyProperty PrincipalProperty =
      DependencyProperty.Register("Principal",
                                  typeof(IPrincipal),
                                  typeof(AuthorizationContentLoader),
                                  new PropertyMetadata(null));


    /// <summary>
    ///   The <see cref="INavigationAuthorizer"/> that will be used to authorize the <see cref="IPrincipal"/>.
    /// </summary>
    public INavigationAuthorizer Authorizer
    {
      get { return (INavigationAuthorizer)GetValue(AuthorizerProperty); }
      set { SetValue(AuthorizerProperty, value); }
    }

    /// <summary>
    ///   The <see cref="INavigationContentLoader"/> being wrapped by the <see cref="AuthorizationContentLoader"/>.
    /// </summary>
    public INavigationContentLoader ContentLoader
    {
      get { return (INavigationContentLoader)GetValue(ContentLoaderProperty); }
      set { SetValue(ContentLoaderProperty, value); }
    }

    /// <summary>
    ///   The <see cref="IPrincipal"/> that will be used to check authorization.
    /// </summary>
    public IPrincipal Principal
    {
      get { return (IPrincipal)GetValue(PrincipalProperty); }
      set { SetValue(PrincipalProperty, value); }
    }


    /// <summary>
    ///   Gets a value that indicates whether the specified URI can be loaded.
    /// </summary>
    /// <param name = "targetUri">The URI to test.</param>
    /// <param name = "currentUri">The URI that is currently loaded.</param>
    /// <returns><c>true</c> if the URI can be loaded; otherwise, <c>false</c>.</returns>
    public override bool CanLoad(Uri targetUri, Uri currentUri)
    {
      return (ContentLoader ?? _defaultLoader).CanLoad(targetUri, currentUri);
    }

    /// <summary>
    ///   Creates an instance of a <see cref="LoaderBase"/> that will be used to handle loading.
    /// </summary>
    /// <returns>An instance of a <see cref="LoaderBase"/>.</returns>
    protected override LoaderBase CreateLoader()
    {
      return new AuthorizationLoader(this);
    }

    #region Nested type: AuthorizationLoader

    private class AuthorizationLoader : LoaderBase
    {
      private readonly AuthorizationContentLoader _parent;
      private INavigationContentLoader _loader;
      private IAsyncResult _result;

      public AuthorizationLoader(AuthorizationContentLoader parent)
      {
        _parent = parent;
      }

      public override void Cancel()
      {
        _loader.CancelLoad(_result);
      }

      public override void Load(Uri targetUri, Uri currentUri)
      {
        _loader = _parent.ContentLoader ?? _defaultLoader;
        if (_parent.Authorizer != null)
        {
          try
          {
            _parent.Authorizer.CheckAuthorization(_parent.Principal, targetUri, currentUri);
          }
          catch (Exception e)
          {
            Error(e);
            return;
          }
        }
        try
        {
          _result = _loader.BeginLoad(targetUri, currentUri, res =>
          {
            try
            {
              var loadResult = _loader.EndLoad(res);
              if (loadResult.RedirectUri != null)
                Complete(loadResult.RedirectUri);
              else
                Complete(loadResult.LoadedContent);

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
    }

    #endregion
  }
}