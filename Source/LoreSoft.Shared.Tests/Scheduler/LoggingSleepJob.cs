using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LoreSoft.Shared.Scheduler;
using System.Threading;

namespace LoreSoft.Shared.Tests.Scheduler
{
    public class LoggingSleepJob : JobBase
    {

        public override JobResult Run(JobContext context)
        {
            Console.WriteLine("Job is running on Thread#: {0}", Thread.CurrentThread.ManagedThreadId);

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

        public override void Cancel()
        {
            Console.WriteLine("Job is canceling on Thread#: {0}", Thread.CurrentThread.ManagedThreadId);

            base.Cancel();
        }
    }
}
