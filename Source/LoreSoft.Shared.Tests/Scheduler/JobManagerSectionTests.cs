using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using LoreSoft.Shared.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoreSoft.Shared.Tests.Scheduler
{
    [TestClass]
    public class JobManagerSectionTests
    {
        

        [TestMethod]
        public void Load()
        {
            var jobManager = ConfigurationManager.GetSection("jobManager") as JobManagerSection;

            Assert.IsNotNull(jobManager);
        }
    }
}
