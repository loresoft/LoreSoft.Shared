using System;

namespace LoreSoft.Shared.ComponentModel
{
    /// <summary>
    /// The base class for pipeline modules
    /// </summary>
    /// <typeparam name="TContext">The type of the pipeline context.</typeparam>
    public abstract class PipelineActionBase<TContext>
      where TContext : PipelineContextBase
    {
        /// <summary>
        /// Gets the name of this pipeline module.
        /// </summary>
        /// <value>The pipeline module name.</value>
        public virtual string Name
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the priority of this pipeline module.
        /// </summary>
        /// <value>The pipeline module priority.</value>
        public virtual int Priority
        {
            get { return 10; }
        }

        /// <summary>
        /// Processes this module using the specified pipeline context.
        /// </summary>
        /// <param name="context">The pipeline context.</param>
        public abstract void Process(TContext context);
    }
}