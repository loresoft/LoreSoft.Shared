using System;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///   An INavigationContentLoader that redirects to an error page rather than loading it in place.
  /// </summary>
  public class ErrorRedirector : ContentLoaderBase
  {
    /// <summary>
    ///   Creates an instance of a LoaderBase that will be used to handle loading.
    /// </summary>
    /// <returns>An instance of a LoaderBase.</returns>
    protected override LoaderBase CreateLoader()
    {
      return new Loader();
    }

    #region Nested type: Loader

    private class Loader : LoaderBase
    {
      public override void Cancel()
      {
        return;
      }

      public override void Load(Uri targetUri, Uri currentUri)
      {
        Complete(targetUri);
      }
    }

    #endregion
  }
}