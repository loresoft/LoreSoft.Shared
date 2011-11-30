using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoreSoft.Shared.ComponentModel;

namespace LoreSoft.Shared.Tests.ComponentModel
{
  [TestClass]
  public class ValidationBaseTest
  {
    [TestMethod]
    public void WhenNotifyingOnAnInstancePropertyThenAnEventIsFired()
    {
      var testObject = new TestValidationObject();
      var changeTracker = new PropertyChangeTracker(testObject);

      testObject.InstanceProperty = "newValue";

      Assert.IsTrue(changeTracker.ChangedProperties.Contains("InstanceProperty"));
    }
    
#if SILVERLIGHT
    [TestMethod]
    public void WhenValidationOnAnInstancePropertyWithEmptyError()
    {
      var testObject = new TestValidationObject();
      var changeTracker = new PropertyChangeTracker(testObject);

      testObject.InstanceProperty = null;

      Assert.IsTrue(changeTracker.ChangedProperties.Contains("InstanceProperty"));
      Assert.IsInstanceOfType(testObject, typeof(INotifyDataErrorInfo));

      var errorInfo = (INotifyDataErrorInfo)testObject;
      var list = errorInfo.GetErrors("InstanceProperty").Cast<string>().ToList();
      CollectionAssert.Contains(list, "The InstanceProperty field is required.");
    }


    [TestMethod]
    public void WhenValidationOnMaxPropertyErrorThenNoError()
    {
      var testObject = new TestValidationObject();
      var changeTracker = new PropertyChangeTracker(testObject);

      testObject.MaxValue = 0;

      Assert.IsTrue(changeTracker.ChangedProperties.Contains("MaxValue"));
      Assert.IsInstanceOfType(testObject, typeof(INotifyDataErrorInfo));

      var errorInfo = (INotifyDataErrorInfo)testObject;
      var list = errorInfo.GetErrors("MaxValue").Cast<string>().ToList();
      CollectionAssert.Contains(list, "The field MaxValue must be between 1 and 10.");

      changeTracker.Reset();
      testObject.MaxValue = 2;

      Assert.IsTrue(changeTracker.ChangedProperties.Contains("MaxValue"));
      list = errorInfo.GetErrors("MaxValue").Cast<string>().ToList();
      Assert.AreEqual(0, list.Count);
    }

    [TestMethod]
    public void WhenValidate()
    {
      var testObject = new TestValidationObject();

      Assert.IsFalse(testObject.Validate());
      Assert.IsInstanceOfType(testObject, typeof(INotifyDataErrorInfo));

      var errorInfo = (INotifyDataErrorInfo)testObject;
      var list1 = errorInfo.GetErrors("MaxValue").Cast<string>().ToList();
      CollectionAssert.Contains(list1, "The field MaxValue must be between 1 and 10.");
      var list2 = errorInfo.GetErrors("InstanceProperty").Cast<string>().ToList();
      CollectionAssert.Contains(list2, "The InstanceProperty field is required.");

      testObject.InstanceProperty = "test";
      testObject.MaxValue = 2;

      Assert.IsTrue(testObject.Validate());
      list1 = errorInfo.GetErrors("MaxValue").Cast<string>().ToList();
      Assert.AreEqual(0, list1.Count);
      list2 = errorInfo.GetErrors("InstanceProperty").Cast<string>().ToList();
      Assert.AreEqual(0, list2.Count);

    }

#else
    [TestMethod]
    public void WhenValidationOnAnInstancePropertyWithEmptyError()
    {
      var testObject = new TestValidationObject();
      var changeTracker = new PropertyChangeTracker(testObject);

      testObject.InstanceProperty = null;

      Assert.IsTrue(changeTracker.ChangedProperties.Contains("InstanceProperty"));      
      Assert.IsInstanceOfType(testObject, typeof(IDataErrorInfo));

      var errorInfo = (IDataErrorInfo)testObject;
      Assert.AreEqual("The InstanceProperty field is required.", errorInfo["InstanceProperty"]);
    }


    [TestMethod]
    public void WhenValidationOnMaxPropertyErrorThenNoError()
    {
      var testObject = new TestValidationObject();
      var changeTracker = new PropertyChangeTracker(testObject);

      testObject.MaxValue = 0;

      Assert.IsTrue(changeTracker.ChangedProperties.Contains("MaxValue"));
      Assert.IsInstanceOfType(testObject, typeof(IDataErrorInfo));

      var errorInfo = (IDataErrorInfo)testObject;
      Assert.AreEqual("The field MaxValue must be between 1 and 10.", errorInfo["MaxValue"]);

      changeTracker.Reset();
      testObject.MaxValue = 2;

      Assert.IsTrue(changeTracker.ChangedProperties.Contains("MaxValue"));
      Assert.AreEqual(null, errorInfo["MaxValue"]);
    }

    [TestMethod]
    public void WhenValidate()
    {
      var testObject = new TestValidationObject();

      Assert.IsFalse(testObject.Validate());
      Assert.IsInstanceOfType(testObject, typeof(IDataErrorInfo));

      var errorInfo = (IDataErrorInfo)testObject;
      Assert.AreEqual("The field MaxValue must be between 1 and 10.", errorInfo["MaxValue"]);
      Assert.AreEqual("The InstanceProperty field is required.", errorInfo["InstanceProperty"]);

      testObject.InstanceProperty = "test";
      testObject.MaxValue = 2;

      Assert.IsTrue(testObject.Validate());
      Assert.AreEqual(null, errorInfo["MaxValue"]);
      Assert.AreEqual(null, errorInfo["InstanceProperty"]);
    }
#endif
    [TestMethod]
    public void ValidationShouldBeDataContractSerializable()
    {
      var serializer = new DataContractSerializer(typeof(TestValidationObject));
      var stream = new System.IO.MemoryStream();
      bool invoked = false;

      var testObject = new TestValidationObject();
      testObject.PropertyChanged += (o, e) => { invoked = true; };

      serializer.WriteObject(stream, testObject);

      stream.Seek(0, System.IO.SeekOrigin.Begin);

      var reconstitutedObject = serializer.ReadObject(stream) as TestValidationObject;

      Assert.IsNotNull(reconstitutedObject);
    }

    [TestMethod]
    public void ValidationShouldBeXmlSerializable()
    {
      var serializer = new XmlSerializer(typeof(TestValidationObject));

      var writeStream = new System.IO.StringWriter();

      var testObject = new TestValidationObject();

      serializer.Serialize(writeStream, testObject);


      var readStream = new System.IO.StringReader(writeStream.ToString());
      var reconstitutedObject = serializer.Deserialize(readStream) as TestValidationObject;

      Assert.IsNotNull(reconstitutedObject);
    }

#if !SILVERLIGHT
    [TestMethod]
    public void ValidationShouldBeSerializable()
    {
      var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      var stream = new System.IO.MemoryStream();
      bool invoked = false;

      var testObject = new TestValidationObject();
      testObject.PropertyChanged += (o, e) => { invoked = true; };

      serializer.Serialize(stream, testObject);

      stream.Seek(0, System.IO.SeekOrigin.Begin);

      var reconstitutedObject = serializer.Deserialize(stream) as TestValidationObject;

      Assert.IsNotNull(reconstitutedObject);
    }
#endif

#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public class TestValidationObject : ValidationBase
    {
      public TestValidationObject() : base()
      {}

      private string _instanceValue;

      [Required]
      [DataMember]
      public string InstanceProperty
      {
        get { return _instanceValue; }
        set
        {
          ValidateProperty(() => InstanceProperty, value);
          _instanceValue = value;
          RaisePropertyChanged(() => InstanceProperty);
        }
      }

      private int _maxValue;

      [RangeAttribute(1, 10)]
      [DataMember]
      public int MaxValue
      {
        get { return _maxValue; }
        set
        {
          ValidateProperty(() => MaxValue, value);
          _maxValue = value;
          RaisePropertyChanged(() => MaxValue);
        }
      }
    }

  }
}
