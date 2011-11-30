using System;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///   Represents an authorization rule for the <see cref="NavigationAuthorizer"/>
  /// </summary>
  [ContentProperty("Parts")]
  public class AuthorizationRule : DependencyObject
  {
    private const string ErrorStringPattern = "Cannot access due to rule for Uri Pattern: \"{0}\"";

    /// <summary>
    ///   Specifies whether the regular expression will ignore case when checking for matches.  True by default.
    /// </summary>
    public static readonly DependencyProperty IgnoreCaseProperty =
      DependencyProperty.Register("IgnoreCase",
                                  typeof(bool),
                                  typeof(AuthorizationRule),
                                  new PropertyMetadata(true));

    /// <summary>
    ///   The set of parts (e.g. Allow and Deny) that make up the authorization rule.
    /// </summary>
    public static readonly DependencyProperty PartsProperty =
      DependencyProperty.Register("Parts",
                                  typeof(DependencyObjectCollection<IAuthorizationRulePart>),
                                  typeof(AuthorizationRule),
                                  new PropertyMetadata(null));

    /// <summary>
    ///   Specifies a regular expression to be used to match Uris being loaded.
    /// </summary>
    public static readonly DependencyProperty UriProperty =
      DependencyProperty.Register("Uri",
                                  typeof(string),
                                  typeof(AuthorizationRule),
                                  new PropertyMetadata(""));


    /// <summary>
    ///   Constructs a new AuthorizationRule.
    /// </summary>
    public AuthorizationRule()
    {
      Parts = new DependencyObjectCollection<IAuthorizationRulePart>();
    }


    /// <summary>
    ///   Specifies whether the regular expression will ignore case when checking for matches.  True by default.
    /// </summary>
    public bool IgnoreCase
    {
      get { return (bool)GetValue(IgnoreCaseProperty); }
      set { SetValue(IgnoreCaseProperty, value); }
    }

    /// <summary>
    ///   The set of parts (e.g. Allow and Deny) that make up the authorization rule.
    /// </summary>
    public DependencyObjectCollection<IAuthorizationRulePart> Parts
    {
      get { return (DependencyObjectCollection<IAuthorizationRulePart>)GetValue(PartsProperty); }
      set { SetValue(PartsProperty, value); }
    }

    /// <summary>
    ///   Specifies a regular expression to be used to match Uris being loaded.
    /// </summary>
    public string UriPattern
    {
      get { return (string)GetValue(UriProperty); }
      set { SetValue(UriProperty, value); }
    }


    /// <summary>
    ///   Checks the principal against the parts of the rule and throws if the principal is unauthorized.
    /// </summary>
    /// <param name = "principal">The principal whose credentials are being checked.</param>
    public void Check(IPrincipal principal)
    {
      if (Parts == null || Parts.Count == 0)
        return;
      foreach (var rule in Parts)
      {
        if (rule.IsAllowed(principal))
          return;
        if (rule.IsDenied(principal))
          throw new UnauthorizedAccessException(string.Format(ErrorStringPattern, UriPattern));
      }
      throw new UnauthorizedAccessException(string.Format(ErrorStringPattern, UriPattern));
    }

    /// <summary>
    ///   Checks to see whether the given Uri matches the <see cref="UriPattern"/>.
    /// </summary>
    /// <param name = "uri">The Uri being matched.</param>
    /// <returns><c>true</c> if the Uri is a match for the regular expression pattern supplied as <see cref="UriPattern"/>.</returns>
    public bool Matches(Uri uri)
    {
      RegexOptions options = IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
      var regex = new Regex(UriPattern, options);

      return regex.IsMatch(uri.OriginalString);
    }
  }
}