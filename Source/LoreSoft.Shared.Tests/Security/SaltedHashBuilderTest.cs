using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoreSoft.Shared.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoreSoft.Shared.Tests.Security
{
    [TestClass]
    public class SaltedHashBuilderTest
    {
        [TestMethod]
        public void GetHash()
        {
            Guid guid = Guid.NewGuid();
            var builder = new SaltedHashBuilder(guid.ToString());

            builder.Append(true);
            builder.Append('c');
            builder.Append(DateTime.Now);
            builder.Append("this is i a test");

            string hash = builder.GetHash();
        }
    }
}
