using System;
using System.Numerics;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoreSoft.Shared.Text;

namespace LoreSoft.Shared.Tests.Text
{
  /// <summary>
  /// Summary description for BaseConvertTest
  /// </summary>
  [TestClass]
  public class BaseConvertTest
  {
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void TestMethod5()
    {
      Guid guid = new Guid("{d3902802-2842-4d50-bc5b-134c27efe5ff}");
      Console.WriteLine(guid);

      byte[] guidBytes = guid.ToByteArray();
      byte[] sifted = new byte[guidBytes.Length + 1];
      Buffer.BlockCopy(guidBytes, 0, sifted, 0, guidBytes.Length);

      BigInteger bigInteger = new BigInteger(sifted);
      Assert.AreEqual(BigInteger.One, bigInteger.Sign);

      string encoded = BaseConvert.ToBaseString(bigInteger, BaseConvert.Base62);
      Assert.IsNotNull(encoded);
      Console.WriteLine(encoded);

      BigInteger result = BaseConvert.FromBaseString(encoded, BaseConvert.Base62);
      Assert.IsNotNull(result);

      byte[] actualBytes = result.ToByteArray();
      byte[] bytes = new byte[16];

      int count = Math.Min(bytes.Length, actualBytes.Length);
      Buffer.BlockCopy(actualBytes, 0, bytes, 0, count);

      Guid actual = new Guid(bytes);
      Assert.AreEqual(guid, actual);
      Console.WriteLine(actual);
    }

  }
}
