using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#if SILVERLIGHT
using LoreSoft.Shared.Collections;
#else
using System.Collections.Concurrent;
#endif

namespace LoreSoft.Shared.ComponentModel
{
    /// <summary>
    /// The base class for a pipeline service.
    /// </summary>
    /// <typeparam name="TContext">The type used as the context for the pipeline.</typeparam>
    /// <typeparam name="TModule">The base type of the pipeline module to run in this pipeline.</typeparam>
    /// <remarks>
    /// The pipeline works by collection modules (classes) that have a common base class to run in a series.
    /// To setup a pipeline, you have to have a context class that will hold all the common data for the pipeline.
    /// You also have to have a common base class that inherits <see cref="T:LoreSoft.Shared.ComponentModel.PipelineActionBase`1"/> 
    /// for all your modules. The pipeline looks for all types that inherit that common base class to run.
    /// </remarks>
    public abstract class PipelineBase<TContext, TModule>
        where TModule : PipelineActionBase<TContext>
        where TContext : PipelineContextBase
    {
        private static readonly ConcurrentDictionary<Type, List<TModule>> _moduleCache;
        private int _runningCount;

        static PipelineBase()
        {
            _moduleCache = new ConcurrentDictionary<Type, List<TModule>>();
        }

        /// <summary>
        /// Gets a value indicating whether this pipeling is running.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get { return _runningCount > 0; }
        }

        /// <summary>
        /// Gets the modules to run for the pipeline.
        /// </summary>
        public IEnumerable<TModule> Modules
        {
            get { return GetModules(); }
        }

        /// <summary>
        /// Runs all the modules of pipeline with the specified context list.
        /// </summary>
        /// <param name="contexts">The context list to run the modules with.</param>
        public virtual void Run(IEnumerable<TContext> contexts)
        {
            if (Interlocked.Exchange(ref _runningCount, 1) != 0)
                return;

            try
            {
                var modules = GetModules();
                foreach (var context in contexts)
                    Run(context, modules);
            }
            finally
            {
                Interlocked.Exchange(ref _runningCount, 0);
            }
        }

        /// <summary>
        /// Runs all the modules of pipeline with the specified context.
        /// </summary>
        /// <param name="context">The context to run the modules with.</param>
        public virtual void Run(TContext context)
        {
            Interlocked.Increment(ref _runningCount);

            try
            {
                var modules = GetModules();
                Run(context, modules);
            }
            finally
            {
                Interlocked.Decrement(ref _runningCount);
            }
        }

        /// <summary>
        /// Runs all the specified modules of pipeline with the specified context.
        /// </summary>
        /// <param name="context">The context to run the modules with.</param>
        /// <param name="modules">The list modules to run.</param>
        protected virtual void Run(TContext context, IEnumerable<TModule> modules)
        {
            OnStarting(context);
            foreach (TModule module in modules)
            {
                if (OnProcessing(context, module))
                    module.Process(context);
                if (context.IsCancelled)
                    break;
            }
            OnCompleted(context);
        }

        /// <summary>
        /// Called before any pipeline modules are run.
        /// </summary>
        /// <param name="context">The context the modules will run with.</param>
        protected virtual void OnStarting(TContext context)
        { }

        /// <summary>
        /// Called before processing a pipeline module.
        /// </summary>
        /// <param name="context">The context the modules will run with.</param>
        /// <param name="module">The module.</param>
        /// <returns>Return <c>true</c> to process the module; otherwise <c>false</c> to skip the module.</returns>
        protected virtual bool OnProcessing(TContext context, TModule module)
        {
            return true;
        }

        /// <summary>
        /// Called after all pipeline modules have run.
        /// </summary>
        /// <param name="context">The context the modules ran with.</param>
        protected virtual void OnCompleted(TContext context)
        { }

        /// <summary>
        /// Gets the modules that are subclasses of <typeparamref name="TModule"/>.
        /// </summary>
        /// <returns>An IEnumerable of modules to run for the pipeline.</returns>
        protected static IEnumerable<TModule> GetModules()
        {
            return _moduleCache.GetOrAdd(typeof(TModule), t =>
            {
                var modules = (from type in t.Assembly.GetTypes()
                               where (type.IsClass && !type.IsNotPublic)
                                    && !type.IsAbstract
                                    && type.IsSubclassOf(t)
                               select Activator.CreateInstance(type))
                               .OfType<TModule>()
                               .OrderBy(r => r.Priority)
                               .ToList();
                return modules;
            });
        }
    }
}
