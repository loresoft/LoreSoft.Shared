using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LoreSoft.Shared.Scheduler;
using System.Threading;

namespace LoreSoft.Shared.Tests.Scheduler
{
    public class SleepJob : JobBase
    {

        public override JobResult Run(JobContext context)
        {
            JobResult r = new JobResult();
            int sleep = 5;
            if (context.Arguments.ContainsKey("sleep"))
                sleep = (int) context.Arguments["sleep"];

            TimeSpan timeSpan = TimeSpan.FromSeconds(sleep);
            string message = string.Format("Sleep for {0} sec start.", timeSpan.TotalSeconds);
            
            context.UpdateStatus(message);
            Debug.WriteLine(message);
            
            Thread.Sleep(sleep);
            r.Result = string.Format("Sleep for {0} sec Complete.", timeSpan.TotalSeconds); ;

            return r;
        }
    }
}
