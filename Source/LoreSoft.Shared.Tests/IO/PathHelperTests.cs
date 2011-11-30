using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LoreSoft.Shared.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoreSoft.Shared.Tests.IO
{
    [TestClass]
    public class PathHelperTests
    {

#if !SILVERLIGHT
        [TestMethod]
        public void GetUniqueName()
        {
            string p = PathHelper.GetUniqueName(@"IO\Document.txt");
            Assert.AreEqual(@"IO\Document[1].txt", @"IO\Document[1].txt");

            p = PathHelper.GetUniqueName(@"IO\Document - Copy.txt");
            Assert.AreEqual(@"IO\Document - Copy[3].txt", @"IO\Document - Copy[3].txt");
        }

        [TestMethod]
        public void GetCleanName()
        {
            string p = PathHelper.GetCleanPath(@"IO\Document.txt");
            Assert.AreEqual(@"IO\Document.txt", p);

            p = PathHelper.GetCleanPath(@"IO\<Document>.txt");
            Assert.AreEqual(@"IO\Document.txt", p);

            p = PathHelper.GetCleanPath(@"IO\Doc|ument.txt");
            Assert.AreEqual(@"IO\Document.txt", p);

            p = PathHelper.GetCleanPath(@"IO\Doc|ument.txt");
            Assert.AreEqual(@"IO\Document.txt", p);

        }
#endif

        [TestMethod]
        public void Combine()
        {
            string path = PathHelper.Combine("p1", "p2", "p3");
            Assert.AreEqual(@"p1\p2\p3", path);

            path = PathHelper.Combine("p1");
            Assert.AreEqual(@"p1", path);

            path = PathHelper.Combine("p1", null, "p3");
            Assert.AreEqual(@"p1\p3", path);

            path = PathHelper.Combine("p1", string.Empty, "p3");
            Assert.AreEqual(@"p1\p3", path);

            path = PathHelper.Combine(@"c:\p1", @"p2\p4", "p3");
            Assert.AreEqual(@"c:\p1\p2\p4\p3", path);

            path = PathHelper.Combine(@"c:\p1", 123, Guid.Empty);
            Assert.AreEqual(@"c:\p1\123\00000000-0000-0000-0000-000000000000", path);
        }

#if !SILVERLIGHT
        [TestMethod]
        public void DataMacroWithSeperator()
        {
            string expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.xml");
            string path = @"|DataDirectory|\data.xml";
            string r = PathHelper.ExpandPath(path);

            Console.WriteLine(r);
            Assert.AreEqual(expected, r);
        }

        [TestMethod]
        public void DataMacroWithoutSeperator()
        {
            string expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.xml");
            string path = @"|DataDirectory|data.xml";
            string r = PathHelper.ExpandPath(path);
            
            Console.WriteLine(r);
            Assert.AreEqual(expected, r);
        }

        [TestMethod]
        public void DataMacroOnly()
        {
            string expected = AppDomain.CurrentDomain.BaseDirectory;
            string path = @"|DataDirectory|";
            string r = PathHelper.ExpandPath(path);
            
            Console.WriteLine(r);
            Assert.AreEqual(expected, r);
        }

        [TestMethod]
        public void DataMacroOnlySeperator()
        {
            string expected = AppDomain.CurrentDomain.BaseDirectory;
            string path = @"|DataDirectory|\";
            string r = PathHelper.ExpandPath(path);
            
            Console.WriteLine(r);
            Assert.AreEqual(expected, r);
        }

        [TestMethod]
        public void NoDataMacro()
        {
            string expected = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.xml");
            string path = @"data.xml";
            string r = PathHelper.ExpandPath(path);

            Console.WriteLine(r);
            Assert.AreEqual(expected, r);
        }
#endif
    }
}
