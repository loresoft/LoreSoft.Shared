using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoreSoft.Shared.Command;

namespace LoreSoft.Shared.Tests.Commands
{
  [TestClass]
  public class RelayCommandTests
  {
    [TestMethod]
    public void CommandExecutes()
    {
      bool executed = false;
      var target = new RelayCommand(() => executed = true);
      target.Execute(null);
      Assert.IsTrue(executed);
    }

    [TestMethod]
    public void CanExecuteReturnsFalse()
    {
      var target = new RelayCommand(Console.WriteLine, () => false);
      bool result = target.CanExecute(null);
      Assert.IsFalse(result);
    }

    [TestMethod]
    public void ReceiveCorrectParameter()
    {
      bool canExecuteGotParam = false;
      bool executeGotParam = false;

      string paramValue = "whatever";

      var target = new RelayCommand<string>(
          (param) => executeGotParam = (param == paramValue),
          (param) => canExecuteGotParam = (param == paramValue));

      target.CanExecute(paramValue);
      target.Execute(paramValue);

      Assert.IsTrue(canExecuteGotParam);
      Assert.IsTrue(executeGotParam);
    }

  }
}
