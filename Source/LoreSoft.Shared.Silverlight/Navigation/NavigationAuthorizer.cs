using System;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///   A default authorizer for the AuthContentLoader that mimics the behavior of ASP.NET's web.config authorization markup.
  /// </summary>
  [ContentProperty("Rules")]
  public class NavigationAuthorizer : DependencyObject, INavigationAuthorizer
  {

    /// <summary>
    ///   The set of rules used by the authorizer.
    /// </summary>
    public static readonly DependencyProperty RulesProperty =
        DependencyProperty.Register("Rules",
                                    typeof(DependencyObjectCollection<AuthorizationRule>),
                                    typeof(NavigationAuthorizer),
                                    new PropertyMetadata(null));



    /// <summary>
    ///   Constructs a NavigationAuthorizer.
    /// </summary>
    public NavigationAuthorizer()
    {
      this.Rules = new DependencyObjectCollection<AuthorizationRule>();
    }



    /// <summary>
    ///   The set of rules used by the authorizer.
    /// </summary>
    public DependencyObjectCollection<AuthorizationRule> Rules
    {
      get { return (DependencyObjectCollection<AuthorizationRule>)this.GetValue(RulesProperty); }
      set { this.SetValue(RulesProperty, value); }
    }




    #region INavigationAuthorizer Members

    /// <summary>
    ///   Checks whether the principal has sufficient authorization to access the Uri being loaded by the AuthContentLoader.
    ///   If the principal is authorized, this method should simply return.  Otherwise, it should throw.
    /// </summary>
    /// <param name = "principal">The user credentials against which to check.</param>
    /// <param name = "targetUri">The Uri being loaded.</param>
    /// <param name = "currentUri">The current Uri from which the new Uri is being loaded.</param>
    public void CheckAuthorization(IPrincipal principal, Uri targetUri, Uri currentUri)
    {
      foreach (var rule in this.Rules.Where(rule => rule.Matches(targetUri)))
      {
        rule.Check(principal);
        return;
      }
    }

    #endregion
  }
}
