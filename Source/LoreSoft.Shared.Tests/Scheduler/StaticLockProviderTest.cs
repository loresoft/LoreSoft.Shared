using System;
using System.Collections.Generic;
using System.Text;
using LoreSoft.Shared.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoreSoft.Shared.Tests.Scheduler
{
    [TestClass]
    public class StaticLockProviderTest
    {
       
        [TestMethod]
        public void Test()
        {
            StaticLockProvider provider = new StaticLockProvider();
            var v1 = provider.Acquire("Test");
            Console.WriteLine(v1);
            Assert.IsTrue(v1.LockAcquired);
            
            var v2 = provider.Acquire("Test");
            Console.WriteLine(v2);
            Assert.IsFalse(v2.LockAcquired);

            v1.Dispose();

            var v3 = provider.Acquire("Test");
            Console.WriteLine(v3);
            Assert.IsTrue(v3.LockAcquired);

        }
    }
}
