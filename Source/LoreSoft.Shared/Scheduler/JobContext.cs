using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LoreSoft.Shared.Scheduler
{
    public class JobContext : MarshalByRefObject
    {
        private readonly IDictionary<string, object> _arguments;
        private readonly string _description;
        private readonly string _name;
        private readonly Action<string> _updateStatus;
        private readonly DateTime _lastRunTime;
        private readonly JobStatus _lastStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.MarshalByRefObject" /> class. 
        /// </summary>
        public JobContext(string name, string description, DateTime lastRunTime, JobStatus lastStatus, IDictionary<string, object> arguments, Action<string> updateStatus)
        {
            _updateStatus = updateStatus;
            _name = name;
            _description = description;
            _lastRunTime = lastRunTime;
            _lastStatus = lastStatus;
            _arguments = arguments;
        }

        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        /// <value>The name of the job.</value>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the description for the job.
        /// </summary>
        /// <value>The description for the job.</value>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public IDictionary<string, object> Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        /// Gets the last status.
        /// </summary>
        /// <value>The last status.</value>
        public JobStatus LastStatus
        {
            get { return _lastStatus; }
        }

        /// <summary>
        /// Gets the last run time.
        /// </summary>
        /// <value>The last run time.</value>
        public DateTime LastRunTime
        {
            get { return _lastRunTime; }
        }

        public Action<string> ProgressAction
        {
            get { return _updateStatus; }
        }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="message">The message.</param>
        public void UpdateStatus(string message)
        {
            if (_updateStatus != null)
                _updateStatus(message);
        }
    }
}