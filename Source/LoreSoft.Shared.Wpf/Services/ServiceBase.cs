using System;
using LoreSoft.Shared.ComponentModel;
using LoreSoft.Shared.Diagnostics;

#if SILVERLIGHT
using System.Windows;
#else
using LoreSoft.Shared.Windows;
#endif

namespace LoreSoft.Shared.Services
{
  /// <summary>
  /// A base class for application extension service.
  /// </summary>
  /// <typeparam name="TService">The type of service.</typeparam>
  public abstract class ServiceBase<TService>
    : NotificationBase, IApplicationService, IApplicationLifetimeAware
    where TService : ServiceBase<TService>, new()
  {
    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    /// <value>
    /// The name of the service.
    /// </value>
    public abstract string ServiceName { get; }

    private static TService _current;

    /// <summary>
    /// Gets the current singleton instance of this service.
    /// </summary>
    public static TService Current
    {
      get { return _current ?? (_current = new TService()); }
    }

    /// <summary>
    /// Called by an application in order to initialize the application extension service.
    /// </summary>
    /// <param name="context">Provides information about the application state.</param>
    public virtual void StartService(ApplicationServiceContext context)
    {
      _current = (TService)this;

      // added to allow for xaml binding.
      Application.Current.Resources.Add(ServiceName, this);
      Logger<TService>.Info("Service '{0}' Started.", ServiceName);
    }

    /// <summary>
    /// Called by an application in order to stop the application extension service.
    /// </summary>
    public virtual void StopService()
    {
      _current = null;
      Dispose();
    }

    /// <summary>
    /// Called by an application immediately before the Application.Startup event occurs.
    /// </summary>
    public virtual void Starting()
    {

    }

    /// <summary>
    /// Called by an application immediately after the Application.Startup event occurs.
    /// </summary>
    public virtual void Started()
    {

    }

    /// <summary>
    /// Called by an application immediately before the Application.Exit event occurs.
    /// </summary>
    public virtual void Exiting()
    {

    }

    /// <summary>
    /// Called by an application immediately after the Application.Exit event occurs.
    /// </summary>
    public virtual void Exited()
    {

    }
  }
}
