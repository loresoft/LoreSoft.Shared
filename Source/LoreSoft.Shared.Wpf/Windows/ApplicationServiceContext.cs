using System.Collections.Generic;

namespace LoreSoft.Shared.Windows
{
  /// <summary>
  /// Represents the initial state of an application when application extension services are started. 
  /// </summary>
  /// <remarks>
  /// This class is added for Silverlight compatibility.
  /// </remarks>
  public class ApplicationServiceContext
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationServiceContext"/> class.
    /// </summary>
    internal ApplicationServiceContext()
    {
      _applicationInitParams = new Dictionary<string, string>();
    }

    private readonly Dictionary<string, string> _applicationInitParams;
    /// <summary>
    /// Gets the initialization parameters specified.
    /// </summary>
    public Dictionary<string, string> ApplicationInitParams
    {
      get { return _applicationInitParams; }
    }
  }
}