using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LoreSoft.Shared.Scheduler;

namespace LoreSoft.Shared.Tests.Scheduler
{
    public class StaticHistoryProvider : JobHistoryProvider
    {
        public string LastResult { get; set; }
        public DateTime LastRunTime { get; set; }
        public JobStatus LastStatus { get; set; }

        public int RestoreCount { get; private set; }
        public int SaveCount { get; private set; }

        public override void RestoreHistory(JobRunner jobRunner)
        {
            RestoreCount++;

            jobRunner.LastResult = LastResult;
            jobRunner.LastRunStartTime = LastRunTime;
            jobRunner.LastStatus = LastStatus;
        }

        public override void SaveHistory(JobRunner jobRunner)
        {
            SaveCount++;

            LastResult = jobRunner.LastResult;
            LastRunTime = jobRunner.LastRunStartTime;
            LastStatus = jobRunner.LastStatus;
        }
    }
}
