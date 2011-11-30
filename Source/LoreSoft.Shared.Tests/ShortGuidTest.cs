using System;
using System.Diagnostics;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoreSoft.Shared.Tests
{
  /// <summary>
  /// Summary description for ShortGuidTest
  /// </summary>
  [TestClass]
  public class ShortGuidTest
  {
    public TestContext TestContext { get; set; }

    [TestMethod]
    public void Convert()
    {
      Stopwatch watch = Stopwatch.StartNew();
      for (int i = 0; i < 1000000; i++)
      {
        Guid g = Guid.NewGuid();
        ShortGuid s = g;
        string v = s.Value;

        Assert.IsNotNull(v);
        Assert.IsFalse(v.StartsWith("-"));

        ShortGuid s2 = new ShortGuid(v);
        Assert.AreEqual(s, s2);

        Guid g2 = s2;
        Assert.AreEqual(g, g2);
      }
      watch.Stop();
      Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);
    }


    [TestMethod]
    public void ConvertMax()
    {
      Stopwatch watch = Stopwatch.StartNew();
      Guid g = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
      ShortGuid s = g;
      string v = s.Value;

      ShortGuid s2 = new ShortGuid(v);

      watch.Stop();
      Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);

      Assert.AreEqual(s, s2);

      Guid g2 = s2;
      Assert.AreEqual(g, g2);

    }

    
    [TestMethod]
    public void ConvertZero()
    {
      Stopwatch watch = Stopwatch.StartNew();
      Guid g = Guid.Empty;
      ShortGuid s = g;
      string v = s.Value;

      ShortGuid s2 = new ShortGuid(v);

      watch.Stop();
      Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);

      Assert.AreEqual(s, s2);

      Guid g2 = s2;
      Assert.AreEqual(g, g2);

    }

    [TestMethod]
    public void ConvertEdge1()
    {
      Stopwatch watch = Stopwatch.StartNew();
      Guid g = new Guid("ffffffff-ffff-ffff-ffff-fffffffffffe");
      ShortGuid s = g;
      string v = s.Value;

      ShortGuid s2 = new ShortGuid(v);

      watch.Stop();
      Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);

      Assert.AreEqual(s, s2);

      Guid g2 = s2;
      Assert.AreEqual(g, g2);
    }

    [TestMethod]
    public void ConvertEdge2()
    {
      Stopwatch watch = Stopwatch.StartNew();
      Guid g = new Guid("00000000-0000-0000-0000-000000000001");
      ShortGuid s = g;
      string v = s.Value;

      ShortGuid s2 = new ShortGuid(v);

      watch.Stop();
      Console.WriteLine("Time: {0} ms", watch.ElapsedMilliseconds);

      Assert.AreEqual(s, s2);

      Guid g2 = s2;
      Assert.AreEqual(g, g2);
    }

  }
}
