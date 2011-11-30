using System;
using System.Linq;
using System.Security.Principal;
using System.Windows;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///   Specifies the roles/users that are denied to access a page.
  /// </summary>
  public class Deny : DependencyObject, IAuthorizationRulePart
  {
    /// <summary>
    ///   Gets or sets, in a comma-separated list, the set of roles to be denied.
    /// </summary>
    public static readonly DependencyProperty RolesProperty =
      DependencyProperty.Register("Roles", typeof (string), typeof (Deny), new PropertyMetadata(""));

    /// <summary>
    ///   Gets or sets, in a comma-separated list, the set of users to denied.  "?" indicates anonymous users will be denied.
    ///   "*" indicates that all users will be denied.
    /// </summary>
    public static readonly DependencyProperty UsersProperty =
      DependencyProperty.Register("Users", typeof (string), typeof (Deny), new PropertyMetadata(""));

    /// <summary>
    ///   Gets or sets, in a comma-separated list, the set of roles to be denied.
    /// </summary>
    public string Roles
    {
      get { return (string)GetValue(RolesProperty); }
      set { SetValue(RolesProperty, value); }
    }

    /// <summary>
    ///   Gets or sets, in a comma-separated list, the set of users to denied.  "?" indicates anonymous users will be denied.
    ///   "*" indicates that all users will be denied.
    /// </summary>
    public string Users
    {
      get { return (string)GetValue(UsersProperty); }
      set { SetValue(UsersProperty, value); }
    }

    /// <summary>
    ///   Indicates whether the principal is allowed by this rule part.
    /// </summary>
    /// <param name = "principal">The principal to check.</param>
    /// <returns><c>true</c> if the principal is allowed; <c>false</c> otherwise.</returns>
    public bool IsAllowed(IPrincipal principal)
    {
      return false;
    }

    /// <summary>
    ///   Indicates whether the principal is denied by this rule part.
    /// </summary>
    /// <param name = "principal">The principal to check.</param>
    /// <returns><c>true</c> if the principal is denied;  <c>false</c> otherwise.</returns>
    public bool IsDenied(IPrincipal principal)
    {
      if (Users != null && HasUser(Users, principal))
        return true;

      return principal != null && principal.Identity.IsAuthenticated &&
             Roles != null && HasAnyRole(Roles, principal);
    }

    private static bool HasAnyRole(string roles, IPrincipal principal)
    {
      if (principal == null)
        return false;

      var roleList = from r in roles.Split(new[] {',', ';'})
                     select r.Trim();

      return roleList.Any(principal.IsInRole);
    }

    private static bool HasUser(string users, IPrincipal principal)
    {
      var userList = from u in users.Split(new[] {',', ';'})
                     select u.Trim();

      if (principal == null || !principal.Identity.IsAuthenticated)
        return userList.Contains("?") || userList.Contains("*");

      return userList.Contains("*") || userList.Contains(principal.Identity.Name);
    }
  }
}