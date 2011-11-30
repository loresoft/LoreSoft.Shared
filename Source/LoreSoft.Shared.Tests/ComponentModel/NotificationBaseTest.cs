using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoreSoft.Shared.ComponentModel;

namespace LoreSoft.Shared.Tests.ComponentModel
{
  [TestClass]
  public class NotificationBaseTest
  {
    [TestMethod]
    public void WhenNotifyingOnAnInstancePropertyThenAnEventIsFired()
    {
      var testObject = new TestNotificationObject();
      var changeTracker = new PropertyChangeTracker(testObject);

      testObject.InstanceProperty = "newValue";

      Assert.IsTrue(changeTracker.ChangedProperties.Contains("InstanceProperty"));
    }

    [TestMethod]
    public void NotificationShouldBeDataContractSerializable()
    {
      var serializer = new DataContractSerializer(typeof(TestNotificationObject));
      var stream = new System.IO.MemoryStream();
      bool invoked = false;

      var testObject = new TestNotificationObject();
      testObject.PropertyChanged += (o, e) => { invoked = true; };

      serializer.WriteObject(stream, testObject);

      stream.Seek(0, System.IO.SeekOrigin.Begin);

      var reconstitutedObject = serializer.ReadObject(stream) as TestNotificationObject;

      Assert.IsNotNull(reconstitutedObject);
    }

    [TestMethod]
    public void NotificationShouldBeXmlSerializable()
    {
      var serializer = new XmlSerializer(typeof(TestNotificationObject));

      var writeStream = new System.IO.StringWriter();

      var testObject = new TestNotificationObject();

      serializer.Serialize(writeStream, testObject);


      var readStream = new System.IO.StringReader(writeStream.ToString());
      var reconstitutedObject = serializer.Deserialize(readStream) as TestNotificationObject;

      Assert.IsNotNull(reconstitutedObject);
    }

#if !SILVERLIGHT
    [TestMethod]
    public void NotificationShouldBeSerializable()
    {
      var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      var stream = new System.IO.MemoryStream();
      bool invoked = false;

      var testObject = new TestNotificationObject();
      testObject.PropertyChanged += (o, e) => { invoked = true; };

      serializer.Serialize(stream, testObject);

      stream.Seek(0, System.IO.SeekOrigin.Begin);

      var reconstitutedObject = serializer.Deserialize(stream) as TestNotificationObject;

      Assert.IsNotNull(reconstitutedObject);
    }
#endif

#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public class TestNotificationObject : NotificationBase
    {
      private string instanceValue;

      public string InstanceProperty
      {
        get { return instanceValue; }
        set
        {
          instanceValue = value;
          RaisePropertyChanged(() => InstanceProperty);
        }
      }
    }

  }
}
