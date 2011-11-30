using System;
using System.Security.Principal;

namespace LoreSoft.Shared.Navigation
{
  /// <summary>
  ///   An interface for specifying part of a rule based upon some characteristics of the principal.  The premade instances of this interface are
  ///   Allow and Deny, and work similar to the ASP.NET web.config authentication options.
  /// </summary>
  public interface IAuthorizationRulePart
  {

    /// <summary>
    ///   Indicates whether the principal is allowed by this rule part.
    /// </summary>
    /// <param name = "principal">The principal to check.</param>
    /// <returns>True if the principal is allowed.  False otherwise.</returns>
    bool IsAllowed(IPrincipal principal);

    /// <summary>
    ///   Indicates whether the principal is denied by this rule part.
    /// </summary>
    /// <param name = "principal">The principal to check.</param>
    /// <returns>True if the principal is denied.  False otherwise.</returns>
    bool IsDenied(IPrincipal principal);
  }
}
