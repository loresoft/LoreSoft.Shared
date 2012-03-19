using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace LoreSoft.Shared.Scheduler
{
    /// <summary>
    /// A class to manage the jobs for the Scheduler.
    /// </summary>
    public class JobManager
    {
        private static readonly object _initLock = new object();
        internal static int JobsRunning;

        private readonly string _id;
        private readonly Dictionary<JobProvider, JobCollection> _providerJobs;
        private readonly JobLockProviderCollection _jobLockProviders;
        private readonly JobLockProvider _defaultJobLockProvider;
        private readonly JobProviderCollection _jobProviders;
        private readonly JobCollection _jobs;
        private readonly Timer _jobProviderTimer;
        
        private bool _isInitialized;
        private DateTime _lastInitilize;

        #region Events
        /// <summary>
        /// Occurs when the JobManager starts.
        /// </summary>
        /// <seealso cref="Start"/>
        public event EventHandler<JobEventArgs> JobMangerStarting;

        /// <summary>
        /// Raises the <see cref="E:LoreSoft.Shared.Scheduler.JobManager.JobMangerStarting"/> event.
        /// </summary>
        /// <param name="e">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        private void OnJobMangerStarting(JobEventArgs e)
        {
            if (JobMangerStarting == null)
                return;

            JobMangerStarting(this, e);
        }

        /// <summary>
        /// Occurs when the JobManager stops.
        /// </summary>
        /// <seealso cref="Stop"/>
        public event EventHandler<JobEventArgs> JobMangerStopping;

        /// <summary>
        /// Raises the <see cref="E:LoreSoft.Shared.Scheduler.JobManager.JobMangerStopping"/> event.
        /// </summary>
        /// <param name="e">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        private void OnJobMangerStopping(JobEventArgs e)
        {
            if (JobMangerStopping == null)
                return;

            JobMangerStopping(this, e);
        }

        /// <summary>
        /// Occurs when the Job is starting.
        /// </summary>
        /// <seealso cref="M:LoreSoft.Shared.Scheduler.Job.Start"/>
        public event EventHandler<JobEventArgs> JobStarting;

        /// <summary>
        /// Raises the <see cref="E:LoreSoft.Shared.Scheduler.JobManager.JobStarting"/> event.
        /// </summary>
        /// <param name="e">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        internal void OnJobStarting(JobEventArgs e)
        {
            if (JobStarting == null)
                return;

            JobStarting(this, e);
        }

        /// <summary>
        /// Occurs when the Job is stopping.
        /// </summary>
        /// <seealso cref="M:LoreSoft.Shared.Scheduler.Job.Stop"/>
        public event EventHandler<JobEventArgs> JobStopping;

        /// <summary>
        /// Raises the <see cref="E:LoreSoft.Shared.Scheduler.JobManager.JobStopping"/> event.
        /// </summary>
        /// <param name="e">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        internal void OnJobStopping(JobEventArgs e)
        {
            if (JobStopping == null)
                return;

            JobStopping(this, e);
        }

        /// <summary>
        /// Occurs when the Job is running.
        /// </summary>
        /// <seealso cref="M:LoreSoft.Shared.Scheduler.Job.Run"/>
        public event EventHandler<JobEventArgs> JobRunning;

        /// <summary>
        /// Raises the <see cref="E:LoreSoft.Shared.Scheduler.JobManager.JobRunning"/> event.
        /// </summary>
        /// <param name="e">The <see cref="JobEventArgs"/> instance containing the event data.</param>
        internal void OnJobRunning(JobEventArgs e)
        {
            if (JobRunning == null)
                return;

            JobRunning(this, e);
        }

        /// <summary>
        /// Occurs when the Job run is completed.
        /// </summary>
        /// <seealso cref="M:LoreSoft.Shared.Scheduler.Job.Run"/>
        public event EventHandler<JobCompletedEventArgs> JobCompleted;

        /// <summary>
        /// Raises the <see cref="E:LoreSoft.Shared.Scheduler.JobManager.JobCompleted"/> event.
        /// </summary>
        /// <param name="e">The <see cref="JobCompletedEventArgs"/> instance containing the event data.</param>
        internal void OnJobCompleted(JobCompletedEventArgs e)
        {
            if (JobCompleted == null)
                return;

            JobCompleted(this, e);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="JobManager"/> class.
        /// </summary>
        private JobManager()
        {
            _id = Guid.NewGuid().ToString("N").Substring(0, 10).ToLower();
            _providerJobs = new Dictionary<JobProvider, JobCollection>();
            _jobLockProviders = new JobLockProviderCollection();
            _defaultJobLockProvider = new DefaultJobLockProvider();
            _jobProviders = new JobProviderCollection();
            _jobs = new JobCollection();
            _jobProviderTimer = new Timer(OnJobProviderCallback);
        }

        /// <summary>
        /// Gets the number of active jobs.
        /// </summary>
        /// <value>The number of active jobs.</value>
        public int ActiveJobs
        {
            get { return JobsRunning; }
        }

        /// <summary>
        /// Gets the collection of jobs.
        /// </summary>
        /// <value>The collection of jobs.</value>
        public JobCollection Jobs
        {
            get { return _jobs; }
        }

        /// <summary>
        /// Initializes the jobs for this manager.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
                return;

            // thread safe init
            lock (_initLock)
            {
                if (_isInitialized)
                    return;

                //var jobManager = ConfigurationManager.GetSection("jobManager") as JobManagerSection;
                //if (jobManager == null)
                //    throw new ConfigurationErrorsException("Could not find 'jobManager' section in app.config or web.config file.");

                ////load lock providers
                //ProvidersHelper.InstantiateProviders(jobManager.JobLockProviders, _jobLockProviders, typeof(JobLockProvider));

                ////add config jobs
                //AddJobs(jobManager.Jobs, null);

                //// add job providers
                //ProvidersHelper.InstantiateProviders(jobManager.JobProviders, _jobProviders, typeof(JobProvider));

                //foreach (JobProvider jobProvider in _jobProviders)
                //    AddJobs(jobProvider.GetJobs(), jobProvider);

                //_jobProviderTimer.Change(jobManager.JobProviderPoll, jobManager.JobProviderPoll);

                _lastInitilize = DateTime.Now;
                _isInitialized = true;
            }
        }

        private void OnJobProviderCallback(object state)
        {
            bool wasReloaded = false;

            // make thread safe by making sure this can't run when Initialize is running 
            lock (_initLock)
            {
                foreach (JobProvider provider in _jobProviders)
                {
                    if (!provider.IsReloadRequired(_lastInitilize))
                        continue;

                    Trace.TraceInformation("Reload jobs for provider {0}.", provider.ToString());

                    //reload this provider
                    JobCollection providerJobs;
                    if (!_providerJobs.TryGetValue(provider, out providerJobs))
                    {
                        providerJobs = new JobCollection();
                        _providerJobs.Add(provider, providerJobs);
                    }

                    //remove jobs
                    foreach (JobRunner job in providerJobs)
                    {
                        job.Stop(true);
                        _jobs.Remove(job);
                    }
                    providerJobs.Clear();

                    //add jobs back
                    AddJobs(provider.GetJobs(), provider);
                    wasReloaded = true;

                    foreach (JobRunner job in providerJobs)
                        job.Start();
                }
            }

            if (wasReloaded)
                _lastInitilize = DateTime.Now;
        }

        private void AddJobs(IEnumerable<IJobConfiguration> jobs, JobProvider provider)
        {
            if (jobs == null)
                return;

            foreach (var jobConfiguration in jobs)
            {
                Type jobType = Type.GetType(jobConfiguration.Type, false, true);
                if (jobType == null)
                    throw new ConfigurationErrorsException(
                        string.Format("Could not load type '{0}' for job '{1}'.", 
                            jobConfiguration.Type, jobConfiguration.Name));

                JobLockProvider jobLockProvider = _defaultJobLockProvider;

                if (!string.IsNullOrEmpty(jobConfiguration.JobLockProvider))
                {
                    // first try getting from provider collection
                    jobLockProvider = _jobLockProviders[jobConfiguration.JobLockProvider];
                    if (jobLockProvider == null)
                    {
                        // next, try loading type
                        Type lockType = Type.GetType(jobConfiguration.JobLockProvider, false, true);
                        if (lockType == null)
                            throw new ConfigurationErrorsException(
                                string.Format("Could not load job lock type '{0}' for job '{1}'.", 
                                    jobConfiguration.JobLockProvider, jobConfiguration.Name));

                        jobLockProvider = Activator.CreateInstance(lockType) as JobLockProvider;
                    }

                    // if not found in provider collection or couldn't create type.
                    if (jobLockProvider == null)
                        throw new ConfigurationErrorsException(
                            string.Format("Could not find job lock provider '{0}' for job '{1}'.", jobConfiguration.JobLockProvider, jobConfiguration.Name));
                }

                JobHistoryProvider jobHistoryProvider = null;
                if (!string.IsNullOrEmpty(jobConfiguration.JobHistoryProvider))
                {
                    Type historyType = Type.GetType(jobConfiguration.JobHistoryProvider, false, true);
                    if (historyType == null)
                        throw new ConfigurationErrorsException(
                            string.Format("Could not load job history type '{0}' for job '{1}'.", jobConfiguration.JobHistoryProvider, jobConfiguration.Name));

                    jobHistoryProvider = Activator.CreateInstance(historyType) as JobHistoryProvider;                    
                }

                var j = new JobRunner(jobConfiguration, jobType, jobLockProvider, jobHistoryProvider);
                _jobs.Add(j);

                // keep track of jobs for providers so they can be sync'd later
                if (provider == null)
                    continue;

                JobCollection providerJobs;
                if (!_providerJobs.TryGetValue(provider, out providerJobs))
                {
                    providerJobs = new JobCollection();
                    _providerJobs.Add(provider, providerJobs);
                }
                providerJobs.Add(j);
            }

        }

        /// <summary>
        /// Starts all jobs in this manager.
        /// </summary>
        public void Start()
        {
            Trace.TraceInformation("JobManager.Start called at {0} on Thread {1}.", DateTime.Now, Thread.CurrentThread.ManagedThreadId);
            OnJobMangerStarting(new JobEventArgs("{JobManager}", JobAction.Starting, _id));
            
            Initialize();

            lock (_initLock)
            {
                foreach (JobRunner j in _jobs)
                    j.Start();
            }
        }

        /// <summary>
        /// Stops all jobs in this manager.
        /// </summary>
        public void Stop()
        {
            Trace.TraceInformation("JobManager.Stop called at {0} on Thread {1}.", DateTime.Now, Thread.CurrentThread.ManagedThreadId);
            OnJobMangerStopping(new JobEventArgs("{JobManager}", JobAction.Stopping, _id));
            
            Initialize();

            lock (_initLock)
            {
                foreach (JobRunner j in _jobs)
                    j.Stop(true);
            }

            // safe shutdown
            DateTime timeout = DateTime.Now.AddSeconds(30);
            while (JobsRunning > 0)
            {
                Thread.Sleep(300);
                // timeout
                if (timeout < DateTime.Now)
                    break;
            }
        }

        /// <summary>
        /// Reload by stopping all jobs and reloading configuration. All the jobs will be restarted after reload.
        /// </summary>
        public void Reload()
        {
            Reload(true);
        }

        /// <summary>
        /// Reload by stopping all jobs and reloading configuration.
        /// </summary>
        /// <param name="startAfter">if set to <c>true</c> start the jobs after reload.</param>
        public void Reload(bool startAfter)
        {
            // make sure all jobs are stopped
            Stop();

            // clearing collections
            lock (_initLock)
            {
                _jobLockProviders.Clear();
                _jobProviders.Clear();
                _jobs.Clear();
                _lastInitilize = DateTime.MinValue;
                _isInitialized = false;
            }

            if (startAfter)
                Start();
        }

        public JobCollection GetJobsByGroup(string group)
        {
            var jobs = new JobCollection();

            lock (_initLock)
            {
                foreach (var job in _jobs)
                {
                    if (job.Group == group)
                        jobs.Add(job);
                }
            }

            return jobs;
        }

        #region Singleton

        /// <summary>
        /// Gets the current instance of <see cref="JobManager"/>.
        /// </summary>
        /// <value>The current instance.</value>
        public static JobManager Current
        {
            get { return Nested.Current; }
        }


        private class Nested
        {
            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            static Nested()
            { }

            internal static readonly JobManager Current = new JobManager();
        }

        #endregion
    }
}