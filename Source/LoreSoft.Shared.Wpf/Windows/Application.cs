using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Windows
{
  /// <summary>
  /// Base class to add application extension services to WPF applications.
  /// </summary>
  /// <remarks>
  /// This class is added for Silverlight compatibility.
  /// </remarks>
  public abstract class Application
    : System.Windows.Application
  {
    private List<IApplicationService> _applicationLifetimeObjects;

    private static readonly object _lock = new object();
    protected Application()
    {
      _applicationLifetimeObjects = new List<IApplicationService>();
    }

    /// <summary>
    /// Gets the application extension services that have been registered for this application.
    /// </summary>
    public List<IApplicationService> ApplicationLifetimeObjects
    {
      get
      {
        base.VerifyAccess();
        if (_applicationLifetimeObjects == null)
          _applicationLifetimeObjects = new List<IApplicationService>();

        return _applicationLifetimeObjects;
      }
      set
      {
        _applicationLifetimeObjects = value;
      }
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
      var applicationServiceContext = new ApplicationServiceContext();

      foreach (IApplicationService service in ApplicationLifetimeObjects)
      {
        service.StartService(applicationServiceContext);

        var aware = service as IApplicationLifetimeAware;
        if (aware != null)
          aware.Starting();
      }

      base.OnStartup(e);

      ApplicationLifetimeObjects
        .OfType<IApplicationLifetimeAware>()
        .ForEach(a => a.Started());

    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.</param>
    protected override void OnExit(ExitEventArgs e)
    {
      ApplicationLifetimeObjects
        .OfType<IApplicationLifetimeAware>()
        .ForEach(a => a.Exiting());

      base.OnExit(e);

      foreach (IApplicationService service in ApplicationLifetimeObjects)
      {
        var aware = service as IApplicationLifetimeAware;
        if (aware != null)
          aware.Exited();

        service.StopService();
      }
    }
  }
}