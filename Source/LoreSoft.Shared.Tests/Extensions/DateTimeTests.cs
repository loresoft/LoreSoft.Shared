using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Tests.Extensions
{
    [TestClass]
    public class DateTimeTests
    {
        
        [TestMethod]
        public void ToAge()
        {
            DateTime d1 = new DateTime(2009, 1, 4);

            string result = d1.ToAgeString();


            d1 = new DateTime(2008, 6, 19);

            result = d1.ToAgeString();

        }


        [TestMethod]
        public void ToAge2()
        {
            DateTime d1 = new DateTime(2009, 1, 4);

            string result = d1.ToAgeString(1);


            d1 = new DateTime(2008, 6, 19);

            result = d1.ToAgeString(1);

        }

        [TestMethod]
        public void ToBinary()
        {
            DateTime d1 = new DateTime(2009, 1, 4);
            var a = d1.ToBinary();
            var b = DateTimeExtensions.ToBinary(d1);
            Assert.AreEqual(a, b);

            d1 = new DateTime(2008, 6, 19);
            a = d1.ToBinary();
            b = DateTimeExtensions.ToBinary(d1);
            Assert.AreEqual(a, b);

            d1 = new DateTime(2008, 6, 19, 0, 0, 0, DateTimeKind.Utc);
            a = d1.ToBinary();
            b = DateTimeExtensions.ToBinary(d1);
            Assert.AreEqual(a, b);

            d1 = DateTime.Now;
            a = d1.ToBinary();
            b = DateTimeExtensions.ToBinary(d1);
            Assert.AreEqual(a, b);

            d1 = DateTime.UtcNow;
            a = d1.ToBinary();
            b = DateTimeExtensions.ToBinary(d1);
            Assert.AreEqual(a, b);

            d1 = DateTime.Today;
            a = d1.ToBinary();
            b = DateTimeExtensions.ToBinary(d1);
            Assert.AreEqual(a, b);
        }
    }
}
