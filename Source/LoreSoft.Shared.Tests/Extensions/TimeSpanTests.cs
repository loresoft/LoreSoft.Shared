using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Tests.Extensions
{
    [TestClass]
    public class TimeSpanTests
    {
        [TestMethod]
        public void ToWords()
        {
            TimeSpan value = TimeSpan.FromMilliseconds(100);
            Assert.AreEqual("0.1 seconds", value.ToWords());

            value = TimeSpan.FromMilliseconds(100);
            Assert.AreEqual("0.1s", value.ToWords(true));

            value = TimeSpan.FromMilliseconds(2500);
            Assert.AreEqual("2.5 seconds", value.ToWords());

            value = TimeSpan.FromMilliseconds(2500);
            Assert.AreEqual("2.5s", value.ToWords(true));
            
            value = TimeSpan.FromMilliseconds(16500);
            Assert.AreEqual("16 seconds", value.ToWords());

            value = TimeSpan.FromMilliseconds(16500);
            Assert.AreEqual("16s", value.ToWords(true));

            value = TimeSpan.FromHours(6);
            Assert.AreEqual("6 hours", value.ToWords());

            value = TimeSpan.FromHours(6);
            Assert.AreEqual("6h", value.ToWords(true));

            value = TimeSpan.FromMinutes(186);
            Assert.AreEqual("3 hours 6 minutes", value.ToWords());

            value = TimeSpan.FromMinutes(186);
            Assert.AreEqual("3h 6m", value.ToWords(true));
        }
    }
}
