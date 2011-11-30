using LoreSoft.Shared.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using LoreSoft.Shared.Configuration;

namespace LoreSoft.Shared.Tests
{


  [TestClass()]
  public class ConfigurationDictionaryTest
  {
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void EncryptTest()
    {
      string keyPhrase = "test-key";
      var target = new ConfigurationDictionary();
      target.Add("bool", "true");
      target.Add("int", "123");
      target.Add("date", new DateTime(2000, 1, 1).ToString());
      target.Add("guid", Guid.NewGuid().ToString());

      
      string encrypt = target.Encrypt(keyPhrase);

      var result = new ConfigurationDictionary();
      result.Decrypt(encrypt, keyPhrase);

      Assert.AreNotSame(target, result);
      Assert.AreEqual(target.Count, result.Count);
    }

  }
}
