using System;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///   Matches an exception with a Uri for an error page.
  /// </summary>
  public interface IErrorPage
  {

    /// <summary>
    ///   Maps an exception to a Uri.
    /// </summary>
    /// <param name = "ex">The exception to map.</param>
    /// <returns>The Uri that should be loaded for the given exception.</returns>
    Uri Map(Exception ex);

    /// <summary>
    ///   Checks whether the exception matches this IErrorPage.
    /// </summary>
    /// <param name = "ex">The exception to check.</param>
    /// <returns>
    ///   true if the exception matches.
    /// </returns>
    bool Matches(Exception ex);
  }
}