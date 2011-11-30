using System.Threading;
using LoreSoft.Shared.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LoreSoft.Shared.Tests.Caching
{


  [TestClass]
  public class CacheManagerTest
  {
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void AddTest()
    {
      var cacheManager = new CacheManager(); 
      string key = "key" + DateTime.Now.Ticks;
      object value = "value" + DateTime.Now.Ticks;
      CachePolicy cachePolicy = null; 
      bool isAdded = cacheManager.Add(key, value, cachePolicy);
      Assert.AreEqual(true, isAdded);
      bool doesContain = cacheManager.Contains(key);
      Assert.AreEqual(true, doesContain);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddValueNullTest()
    {
      var cacheManager = new CacheManager();
      string key = "key" + DateTime.Now.Ticks;
      object value = null;
      CachePolicy cachePolicy = null;
      bool isAdded = cacheManager.Add(key, value, cachePolicy);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddKeyNullTest()
    {
      var cacheManager = new CacheManager();
      string key = null;
      object value = "value" + DateTime.Now.Ticks;
      CachePolicy cachePolicy = null;
      bool isAdded = cacheManager.Add(key, value, cachePolicy);
    }

    [TestMethod]
    public void GetTest()
    {
      var cacheManager = new CacheManager();
      string key = "key" + DateTime.Now.Ticks;
      object value = "value" + DateTime.Now.Ticks;
      CachePolicy cachePolicy = null;
      bool isAdded = cacheManager.Add(key, value, cachePolicy);
      Assert.AreEqual(true, isAdded);
      bool doesContain = cacheManager.Contains(key);
      Assert.AreEqual(true, doesContain);
      var result = cacheManager.Get(key);
      Assert.AreEqual(value, result);
    }

    [TestMethod]
    public void GetOrAddValueFactoryTest()
    {
      var cacheManager = new CacheManager(); 
      string key = "key" + DateTime.Now.Ticks;
      object value = "value" + DateTime.Now.Ticks;
      bool valueCalled = false;

      Func<string, object> valueFactory = k => { valueCalled = true; return value; }; 
      object result = cacheManager.GetOrAdd(key, valueFactory);

      Assert.IsNotNull(result);
      Assert.IsTrue(valueCalled);
      Assert.AreEqual(value, result);

      // value factory 2 should not be called
      object value2 = "value2" + DateTime.Now.Ticks;
      bool value2Called = false;      
      Func<string, object> valueFactory2 = k => { value2Called = true; return value2; };

      object result2 = cacheManager.GetOrAdd(key, valueFactory2);

      Assert.IsNotNull(result2);
      Assert.IsFalse(value2Called);
      // result should still = first call
      Assert.AreEqual(value, result2);
    }

    [TestMethod]
    public void RemoveTest()
    {
      var cacheManager = new CacheManager();
      string key = "key" + DateTime.Now.Ticks;
      object value = "value" + DateTime.Now.Ticks;
      cacheManager.Add(key, value);
      Assert.AreEqual(1, cacheManager.Count);
      var result = cacheManager.Remove(key);
      Assert.AreEqual(value, result);
      Assert.AreEqual(0, cacheManager.Count);
    }

    [TestMethod]
    public void SetTest()
    {
      var cacheManager = new CacheManager(); 
      string key = "key" + DateTime.Now.Ticks;
      object value = "value" + DateTime.Now.Ticks; 
      cacheManager.Set(key, value);
      Assert.AreEqual(1, cacheManager.Count);
    }

    [TestMethod]
    public void ItemTest()
    {
      var cacheManager = new CacheManager();
      string key = "key" + DateTime.Now.Ticks;
      object value = "value" + DateTime.Now.Ticks;
      cacheManager[key] = value;
      object actual = cacheManager[key];
      Assert.AreEqual(value, actual);
    }

    [TestMethod]
    public void ExpireItemTest()
    {
      var cacheManager = new CacheManager();
      string key = "key" + DateTime.Now.Ticks;
      object value = "value" + DateTime.Now.Ticks;
      TimeSpan expiration = TimeSpan.FromSeconds(2);
      
      var cachePolicy = new CachePolicy { AbsoluteExpiration = DateTimeOffset.Now.Add(expiration) };
      
      bool isAdded = cacheManager.Add(key, value, cachePolicy);
      Assert.AreEqual(true, isAdded);
      bool contains = cacheManager.Contains(key);
      Assert.AreEqual(true, contains);
      
      // waited for exiration timer
      Thread.Sleep(TimeSpan.FromSeconds(22));
      contains = cacheManager.Contains(key);
      Assert.AreEqual(false, contains);
      Assert.AreEqual(0, cacheManager.Count);

      cachePolicy = new CachePolicy { SlidingExpiration = expiration };
      isAdded = cacheManager.Add(key, value, cachePolicy);
      Assert.AreEqual(true, isAdded);
      contains = cacheManager.Contains(key);
      Assert.AreEqual(true, contains);

      // waited for exiration timer
      Thread.Sleep(TimeSpan.FromSeconds(22));
      contains = cacheManager.Contains(key);
      Assert.AreEqual(false, contains);
      Assert.AreEqual(0, cacheManager.Count);

    }


    [TestMethod]
    public void UpdatePolicyTest()
    {
      var cacheManager = new CacheManager();
      string key = "key" + DateTime.Now.Ticks;
      object value = "value" + DateTime.Now.Ticks;
      TimeSpan expiration = TimeSpan.FromSeconds(2);

      DateTimeOffset absoluteExpiration = DateTimeOffset.Now.Add(expiration);

      var cachePolicy = new CachePolicy { AbsoluteExpiration = absoluteExpiration };

      bool isAdded = cacheManager.Add(key, value, cachePolicy);
      Assert.AreEqual(true, isAdded);
      bool contains = cacheManager.Contains(key);
      Assert.AreEqual(true, contains);

      var cacheItem = cacheManager.GetCacheItem(key);
      Assert.IsNotNull(cacheItem);
      Assert.AreEqual(absoluteExpiration, cacheItem.AbsoluteExpiration);

      DateTimeOffset newExpiration = DateTimeOffset.Now.AddMinutes(20);
      var newPolicy = new CachePolicy { AbsoluteExpiration = newExpiration };

      cacheManager.Set(key, value, newPolicy);

      var newItem = cacheManager.GetCacheItem(key);

      Assert.IsNotNull(newItem);
      Assert.AreEqual(newExpiration, newItem.AbsoluteExpiration);


    }

  }
}
