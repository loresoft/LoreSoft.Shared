using System.Threading;
using LoreSoft.Shared.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LoreSoft.Shared.Tests.Caching
{


  [TestClass]
  public class CacheItemTest
  {
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void CacheItemConstructorTest()
    {
      string key = "key";
      object value = "value";
      var cachePolicy = new CachePolicy();
      var cacheItem = new CacheItem(key, value, cachePolicy);

      Assert.IsNotNull(cacheItem);
      Assert.AreEqual("key", cacheItem.Key);
      Assert.AreEqual("value", cacheItem.Value);
      Assert.IsNotNull(cacheItem.CachePolicy);
      Assert.AreEqual(CacheManager.NoSlidingExpiration, cacheItem.CachePolicy.SlidingExpiration);
      Assert.AreEqual(CacheManager.InfiniteAbsoluteExpiration, cacheItem.CachePolicy.AbsoluteExpiration);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CacheItemConstructorNullKeyTest()
    {
      string key = null;
      object value = "value";
      var cacheItem = new CacheItem(key, value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CacheItemConstructorNullValueTest()
    {
      string key = "key";
      object value = null;
      var cacheItem = new CacheItem(key, value);
    }


    [TestMethod]
    public void HasExpirationTest()
    {
      string key = "key";
      object value = "value";
      var cachePolicy = new CachePolicy{ SlidingExpiration = TimeSpan.FromSeconds(30)};
      var cacheItem = new CacheItem(key, value, cachePolicy);
      bool hasExpiration = cacheItem.CanExpire();
      Assert.AreEqual(true, hasExpiration);
      
      cachePolicy = new CachePolicy();
      cacheItem = new CacheItem(key, value, cachePolicy);
      hasExpiration = cacheItem.CanExpire();
      Assert.AreEqual(false, hasExpiration);

      cachePolicy = new CachePolicy { AbsoluteExpiration = DateTimeOffset.Now.AddDays(1) };
      cacheItem = new CacheItem(key, value, cachePolicy);
      hasExpiration = cacheItem.CanExpire();
      Assert.AreEqual(true, hasExpiration);
    }

    [TestMethod]
    public void IsExpiredTest()
    {
      string key = "key";
      object value = "value";
      var cachePolicy = new CachePolicy { AbsoluteExpiration = DateTimeOffset.Now.AddDays(1) };
      var cacheItem = new CacheItem(key, value, cachePolicy);
      bool hasExpiration = cacheItem.CanExpire();
      Assert.AreEqual(true, hasExpiration);
      bool isExpired = cacheItem.IsExpired();
      Assert.AreEqual(false, isExpired);

      TimeSpan expiration = TimeSpan.FromSeconds(2);

      cachePolicy = new CachePolicy { SlidingExpiration = expiration };
      cacheItem = new CacheItem(key, value, cachePolicy);
      hasExpiration = cacheItem.CanExpire();
      Assert.AreEqual(true, hasExpiration);
      isExpired = cacheItem.IsExpired();
      Assert.AreEqual(false, isExpired);

      Thread.Sleep(expiration);
      isExpired = cacheItem.IsExpired();
      Assert.AreEqual(true, isExpired);

      cachePolicy = new CachePolicy { AbsoluteExpiration = DateTimeOffset.Now.Add(expiration) };
      cacheItem = new CacheItem(key, value, cachePolicy);
      hasExpiration = cacheItem.CanExpire();
      Assert.AreEqual(true, hasExpiration);
      isExpired = cacheItem.IsExpired();
      Assert.AreEqual(false, isExpired);

      Thread.Sleep(expiration);
      isExpired = cacheItem.IsExpired();
      Assert.AreEqual(true, isExpired);
    }

    [TestMethod]
    public void RaiseExpiredCallbackTest()
    {
      string key = "key";
      object value = "value";
      TimeSpan expiration = TimeSpan.FromSeconds(2);
      bool expireCalled = false;

      var cachePolicy = new CachePolicy
      {
        SlidingExpiration = expiration,
        ExpiredCallback = e => { expireCalled = true; }
      };
      var cacheItem = new CacheItem(key, value, cachePolicy);
      bool hasExpiration = cacheItem.CanExpire();
      Assert.AreEqual(true, hasExpiration);
      bool isExpired = cacheItem.IsExpired();
      Assert.AreEqual(false, isExpired);

      Thread.Sleep(expiration);
      isExpired = cacheItem.IsExpired();
      Assert.AreEqual(true, isExpired);

      cacheItem.RaiseExpiredCallback();
      Thread.Sleep(expiration);

      Assert.AreEqual(true, expireCalled);

    }

    [TestMethod]
    public void UpdateUsageTest()
    {
      string key = "key";
      object value = "value";
      TimeSpan expiration = TimeSpan.FromSeconds(2);

      var cachePolicy = new CachePolicy { SlidingExpiration = expiration };
      var cacheItem = new CacheItem(key, value, cachePolicy);
      
      bool hasExpiration = cacheItem.CanExpire();
      Assert.AreEqual(true, hasExpiration);
      bool isExpired = cacheItem.IsExpired();
      Assert.AreEqual(false, isExpired);

      Thread.Sleep(TimeSpan.FromSeconds(1));
      cacheItem.UpdateUsage();

      isExpired = cacheItem.IsExpired();
      Assert.AreEqual(false, isExpired);

      Thread.Sleep(expiration);
      isExpired = cacheItem.IsExpired();
      Assert.AreEqual(true, isExpired);
      
      cacheItem.UpdateUsage();
      isExpired = cacheItem.IsExpired();
      Assert.AreEqual(false, isExpired);
    }
  }
}
