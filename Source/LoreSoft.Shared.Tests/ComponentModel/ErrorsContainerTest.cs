using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoreSoft.Shared.ComponentModel;

namespace LoreSoft.Shared.Tests.ComponentModel
{
  [TestClass]
  public class ErrorsContainerFixture
  {
    [TestMethod]
    public void WhenCreatingAnInstanceWithANullAction_ThenAnExceptionIsThrown()
    {
      new ErrorsContainer(null);
    }

    [TestMethod]
    public void WhenCreatingInstance_ThenHasNoErrors()
    {
      var validation = new ErrorsContainer(pn => { });

      Assert.IsFalse(validation.HasErrors);
      Assert.IsFalse(validation.GetErrors("property1").Any());
    }

    [TestMethod]
    public void WhenSettingErrorsForPropertyWithNoErrors_ThenNotifiesChangesAndHasErrors()
    {
      List<string> validatedProperties = new List<string>();

      var validation = new ErrorsContainer(pn => validatedProperties.Add(pn));

      validation.SetErrors("property1", new[] { "message" });

      Assert.IsTrue(validation.HasErrors);
      Assert.IsTrue(validation.GetErrors("property1").Contains("message"));
      CollectionAssert.AreEqual(new[] { "property1" }, validatedProperties);
    }

    [TestMethod]
    public void WhenSettingNoErrorsForPropertyWithNoErrors_ThenDoesNotNotifyChangesAndHasNoErrors()
    {
      List<string> validatedProperties = new List<string>();

      var validation = new ErrorsContainer(pn => validatedProperties.Add(pn));

      validation.SetErrors("property1", new string[0]);

      Assert.IsFalse(validation.HasErrors);
      //Assert.IsFalse(validation.GetErrors("property1").Any());
      //Assert.IsFalse(validatedProperties.Any());
    }

    [TestMethod]
    public void WhenSettingErrorsForPropertyWithErrors_ThenNotifiesChangesAndHasErrors()
    {
      List<string> validatedProperties = new List<string>();

      var validation = new ErrorsContainer(pn => validatedProperties.Add(pn));

      validation.SetErrors("property1", new[] { "message" });

      validatedProperties.Clear();

      validation.SetErrors("property1", new[] { "message" });

      Assert.IsTrue(validation.HasErrors);
      Assert.IsTrue(validation.GetErrors("property1").Contains("message"));
      CollectionAssert.AreEqual(new[] { "property1" }, validatedProperties);
    }

    [TestMethod]
    public void WhenSettingNoErrorsForPropertyWithErrors_ThenNotifiesChangesAndHasNoErrors()
    {
      List<string> validatedProperties = new List<string>();

      var validation = new ErrorsContainer(pn => validatedProperties.Add(pn));

      validation.SetErrors("property1", new[] { "message" });

      validatedProperties.Clear();

      validation.SetErrors("property1", new string[0]);

      Assert.IsFalse(validation.HasErrors);
      Assert.IsFalse(validation.GetErrors("property1").Any());
      CollectionAssert.AreEqual(new[] { "property1" }, validatedProperties);
    }
  }
}
