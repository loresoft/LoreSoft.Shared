using System;
using System.Windows;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///   Represents an IErrorPage that redirects to a Uri if given an Exception that matches the ExceptionType,
  ///   or if the ExceptionType is empty, redirects all errors to that Uri.
  /// </summary>
  public sealed class ErrorPage : DependencyObject, IErrorPage
  {
    /// <summary>
    ///   The Uri of the page to load if the exception matches this ErrorPage.
    /// </summary>
    public static readonly DependencyProperty ErrorPageUriProperty =
      DependencyProperty.Register("ErrorPageUri", typeof (Uri), typeof (ErrorPage), new PropertyMetadata(null));

    /// <summary>
    ///   The type name of the exception.
    /// </summary>
    public static readonly DependencyProperty ExceptionTypeProperty =
      DependencyProperty.Register("ExceptionType", typeof (string), typeof (ErrorPage), new PropertyMetadata(null));
    
    /// <summary>
    ///   The Uri of the page to load if the exception matches this ErrorPage.
    /// </summary>
    public Uri ErrorPageUri
    {
      get { return (Uri)GetValue(ErrorPageUriProperty); }
      set { SetValue(ErrorPageUriProperty, value); }
    }

    /// <summary>
    ///   The type name of the exception.
    /// </summary>
    public string ExceptionType
    {
      get { return (string)GetValue(ExceptionTypeProperty); }
      set { SetValue(ExceptionTypeProperty, value); }
    }

    #region IErrorPage Members

    /// <summary>
    ///   Maps an exception to a Uri.
    /// </summary>
    /// <param name = "ex">The exception to map.</param>
    /// <returns>The Uri that should be loaded for the given exception.</returns>
    public Uri Map(Exception ex)
    {
      return ErrorPageUri;
    }

    /// <summary>
    ///   Checks whether the exception matches the ExceptionType.
    /// </summary>
    /// <param name = "ex">The exception to check.</param>
    /// <returns>
    ///   true if the ExceptionType is null or empty, or if the exception inherits from the
    ///   ExceptionType type.  false otherwise.
    /// </returns>
    public bool Matches(Exception ex)
    {
      return string.IsNullOrEmpty(ExceptionType) || Matches(ex.GetType());
    }

    #endregion

    private bool Matches(Type exceptionType)
    {
      return exceptionType.Name.Equals(ExceptionType)
             || exceptionType.FullName.Equals(ExceptionType)
             || (exceptionType.BaseType != null && Matches(exceptionType.BaseType));
    }
  }
}