namespace LoreSoft.Shared.Windows
{
  /// <summary>
  /// Defines methods that application extension services can optionally implement in order to respond to application lifetime events.
  /// </summary>
  /// <remarks>
  /// This interface is added for Silverlight compatibility.
  /// </remarks>
  public interface IApplicationLifetimeAware
  {
    /// <summary>
    /// Called by an application immediately after the Application.Exit event occurs. 
    /// </summary>
    void Exited();
    /// <summary>
    /// Called by an application immediately before the Application.Exit event occurs.
    /// </summary>
    void Exiting();
    /// <summary>
    ///  Called by an application immediately after the Application.Startup event occurs.
    /// </summary>
    void Started();
    /// <summary>
    /// Called by an application immediately before the Application.Startup event occurs.
    /// </summary>
    void Starting();
  }
}