using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using AutoPoco;
using AutoPoco.Configuration;
using AutoPoco.DataSources;
using AutoPoco.Engine;
using LoreSoft.Shared.Reflection;
using LoreSoft.Shared.Tests.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoreSoft.Shared.Tests.Reflection
{
    [TestClass]
    public class ObjectClonerTest
    {
        private readonly IGenerationSessionFactory _factory;

        public ObjectClonerTest()
        {
            _factory = AutoPocoContainer.Configure(x =>
            {
                x.Conventions(c => c.UseDefaultConventions());
                x.AddFromAssemblyContainingType<Task>();
                x.Include<Task>()
                    .Setup(c => c.Id).Value(1)
                    .Setup(c => c.Summary).Use<LoremIpsumSource>()
                    .Setup(c => c.Details).Use<LoremIpsumSource>()
                    .Setup(c => c.CreatedDate).Value(DateTime.Now)
                    .Setup(c => c.ModifiedDate).Value(DateTime.Now);
            });
        }

        [TestMethod]
        public void Clone()
        {
            IGenerationSession session = _factory.CreateSession();
            var task = session.Single<Task>().Get();

            var cloner = new ObjectCloner();
            Assert.IsNotNull(cloner);

            var newTask = cloner.Clone(task);
            Assert.IsNotNull(newTask);

        }

        [TestMethod]
        public void RouteTester()
        {
            var r = new RouteTest();
            r.Dictionary = new RouteValueDictionary(new { Blah = "test" });

            var cloner = new ObjectCloner();
            Assert.IsNotNull(cloner);

            var n = cloner.Clone(r);
            Assert.IsNotNull(n);

        }

        [TestMethod]
        public void IntKeyTester()
        {
            var r = new DictionaryTest();
            r.Dictionary = new IntDictionary();
            r.Dictionary.Add(1, "blah 1");
            r.Dictionary.Add(2, "blah 2");
            r.Dictionary.Add(3, null);


            var cloner = new ObjectCloner();
            Assert.IsNotNull(cloner);

            var n = cloner.Clone(r);
            Assert.IsNotNull(n);

        }

        [TestMethod]
        public void ListTester()
        {
            var r = new ListTest();
            r.List = new List<string>();
            r.List.Add("blah 1");
            r.List.Add("blah 2");
            r.List.Add(null);
            
            var cloner = new ObjectCloner();
            Assert.IsNotNull(cloner);

            var n = cloner.Clone(r) as ListTest;
            Assert.IsNotNull(n);

            Assert.AreNotEqual(r, n);
            Assert.AreNotEqual(r.List, n.List);
        }

        public class RouteTest
        {
            public RouteValueDictionary Dictionary { get; set; }
        }

        public class DictionaryTest
        {
            public IntDictionary Dictionary { get; set; }
        }

        public class IntDictionary : Dictionary<int, string>
        {

        }

        public class ListTest
        {
            public List<string> List { get; set; }
        }
    }

}
