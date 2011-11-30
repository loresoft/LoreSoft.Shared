using System;
using System.Text;

namespace LoreSoft.Shared.Windows
{
  /// <summary>
  /// Defines methods that application extension services must implement in order to enable an application to start and stop the service.
  /// </summary>
  /// <remarks>
  /// This interface is added for Silverlight compatibility.
  /// </remarks>
  public interface IApplicationService
  {
    /// <summary>
    /// Called by an application in order to initialize the application extension service.
    /// </summary>
    /// <param name="context">Provides information about the application state.</param>
    void StartService(ApplicationServiceContext context);
    /// <summary>
    /// Called by an application in order to stop the application extension service.
    /// </summary>
    void StopService();
  }
}
