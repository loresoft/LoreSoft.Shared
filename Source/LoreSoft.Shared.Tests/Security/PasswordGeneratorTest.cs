using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoreSoft.Shared.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoreSoft.Shared.Tests.Security
{
  [TestClass]
  public class PasswordGeneratorTest
  {
    [TestMethod]
    public void GeneratePassword()
    {

      string password = PasswordGenerator.Generate(10);
      Console.WriteLine(password);

      Assert.IsTrue(password.Length == 10);
    }
  }
}
