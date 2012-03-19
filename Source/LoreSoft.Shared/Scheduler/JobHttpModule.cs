using System;
using System.Diagnostics;
using System.Threading;
using System.Web;

namespace CodeSmith.Core.Scheduler
{
    /// <summary>
    /// A Http module class to start the <see cref="JobManager"/>.
    /// </summary>
    public class JobHttpModule : IHttpModule
    {
        private static long _initCount = 0;

        #region IHttpModule Members

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {
            Trace.TraceInformation("JobModule.Dispose called at {0}.", DateTime.Now);
            
            //JobManager.Current.Stop();
        }

        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public void Init(HttpApplication context)
        {
            Trace.TraceInformation("JobModule.Init called at {0}.", DateTime.Now);
            
            // Ensure that this method is called only once (Singleton).
            // Could cause an exception if this is called 4+ billion times.
            if (Interlocked.Increment(ref _initCount) == 1)
            {
                JobManager.Current.Start();
            }
        }

        #endregion

    }
}