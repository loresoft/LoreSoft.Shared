using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LoreSoft.Shared.ComponentModel;
using LoreSoft.Shared.Extensions;
using LoreSoft.Shared.Messaging;
using LoreSoft.Shared.Threading;

namespace LoreSoft.Shared.Tests.Messaging
{
  [TestClass]
  public class MessengerTests
  {
    [TestMethod]
#if SILVERLIGHT
    [ExpectedException(typeof(InvalidOperationException))]
#endif
    public void MessageWithoutParameterIsReceived()
    {
      Messenger target = new Messenger();

      bool received1 = false;
      bool received2 = false;

      target.Subscribe<Message>(m => received1 = true);
      target.Subscribe<Message>(m => received2 = true);

      target.Publish(Message.Empty);

      Assert.IsTrue(received1);
      Assert.IsTrue(received2);
    }

    [TestMethod]
    public void MessageWithoutParameterIsReceivedWrapped()
    {
      Messenger target = new Messenger();
      MessageSubscriber subscriber = new MessageSubscriber(target);

      bool received1 = false;
      bool received2 = false;

      subscriber.Subscribe<Message>(m => received1 = true);
      subscriber.Subscribe<Message>(m => received2 = true);

      target.Publish(Message.Empty);

#if !SILVERLIGHT
      DisplayContext.Current.Dispatcher.DoEvents();
#endif
      Assert.IsTrue(received1);
      Assert.IsTrue(received2);
    }

    [TestMethod]
#if SILVERLIGHT
    [ExpectedException(typeof(InvalidOperationException))]
#endif
    public void MessageWithParameterIsReceived()
    {
      Messenger target = new Messenger();
      TestMessage message = new TestMessage();

      bool received1 = false;
      bool received2 = false;

      target.Subscribe<TestMessage>(m => received1 = (m == message));
      target.Subscribe<TestMessage>(m => received2 = (m == message));

      target.Publish(message);

      Assert.IsTrue(received1);
      Assert.IsTrue(received2);
    }

    [TestMethod]
    public void MessageWithParameterIsReceivedWrapped()
    {
      Messenger target = new Messenger();
      MessageSubscriber subscriber = new MessageSubscriber(target);

      TestMessage message = new TestMessage();

      bool received1 = false;
      bool received2 = false;

      subscriber.Subscribe<TestMessage>(m => received1 = (m == message));
      subscriber.Subscribe<TestMessage>(m => received2 = (m == message));

      target.Publish(message);

#if !SILVERLIGHT
      DisplayContext.Current.Dispatcher.DoEvents();
#endif

      Assert.IsTrue(received1);
      Assert.IsTrue(received2);
    }

    [TestMethod]
    public void UnregisterMessageSubscriber()
    {
      var target = new Messenger();
      var tester = new UnregisterMessageSubscriberTester(target);
      tester.Register();

      target.Publish(Message.Empty);
#if !SILVERLIGHT
      DisplayContext.Current.Dispatcher.DoEvents();
#endif

      Assert.IsTrue(tester.Received);

      tester.Received = false;
      tester.Unregister();

      target.Publish(Message.Empty);
#if !SILVERLIGHT
      DisplayContext.Current.Dispatcher.DoEvents();
#endif

      Assert.IsFalse(tester.Received);
    }

    [TestMethod]
    public void UnregisterToken()
    {
      var target = new Messenger();
      var tester = new UnregisterTokenTester(target);
      tester.Register();

      int c = target.Publish(Message.Empty);
      Assert.AreEqual(2, c);
      Assert.IsTrue(tester.Received1);
      Assert.IsTrue(tester.Received2);

      tester.Received1 = false;
      tester.Received2 = false;
      tester.Unregister1();

      c = target.Publish(Message.Empty);

      Assert.AreEqual(1, c);
      Assert.IsFalse(tester.Received1);
      Assert.IsTrue(tester.Received2);

      tester.Received1 = false;
      tester.Received2 = false;
      tester.Unregister2();

      c = target.Publish(Message.Empty);

      Assert.AreEqual(0, c);
      Assert.IsFalse(tester.Received1);
      Assert.IsFalse(tester.Received2);
    }

    [TestMethod]
    public void DisposeRecipient()
    {
      var target = new Messenger();
      var tester = new DisposeTester(target);
      tester.Register();

      int c = target.Publish(Message.Empty);
      Assert.AreEqual(1, c);

      Assert.IsTrue(tester.Received);

      tester.Received = false;
      tester = null;
      GC.Collect();
      GC.WaitForPendingFinalizers();

      c = target.Publish(Message.Empty);
      Assert.AreEqual(0, c);
    }

    [TestMethod]
    public void DisposeRecipientWrapped()
    {
      var target = new Messenger();
      var subscriber = new MessageSubscriber(target);
      bool received = false;

      subscriber.Subscribe<Message>(m => received = true);

      int c = target.Publish(Message.Empty);
      Assert.AreEqual(1, c);

#if !SILVERLIGHT
      DisplayContext.Current.Dispatcher.DoEvents();
#endif

      Assert.IsTrue(received);

      received = false;
      subscriber = null;
      GC.Collect();
      GC.WaitForPendingFinalizers();

      c = target.Publish(Message.Empty);
      Assert.AreEqual(0, c);
    }

    [TestMethod]
#if SILVERLIGHT
    [ExpectedException(typeof(InvalidOperationException))]
#endif
    public void DisposeAnonymousRecipient()
    {
      var target = new Messenger();
      var tester = new DisposeAnonymousTester(target);
      tester.Register();

      int c = target.Publish(Message.Empty);
      Assert.AreEqual(1, c);

      Assert.IsTrue(tester.Received);

      tester.Received = false;
      tester = null;
      GC.Collect();
      GC.WaitForPendingFinalizers();

      c = target.Publish(Message.Empty);
      Assert.AreEqual(0, c);
    }

    [TestMethod]
#if SILVERLIGHT
    [ExpectedException(typeof(InvalidOperationException))]
#endif
    public void NotifiedInRegistrationOrder()
    {
      Messenger target = new Messenger();

      int notificationCounter = 0;
      int notified1 = 0;
      int notified2 = 0;

      target.Subscribe<Message>(m => notified1 = ++notificationCounter);
      target.Subscribe<Message>(m => notified2 = ++notificationCounter);

      target.Publish(Message.Empty);

      Assert.AreEqual(1, notified1);
      Assert.AreEqual(2, notified2);
    }

    [TestMethod]
    public void NotifiedInRegistrationOrderWrapped()
    {
      Messenger target = new Messenger();
      MessageSubscriber subscriber = new MessageSubscriber(target);

      int notificationCounter = 0;
      int notified1 = 0;
      int notified2 = 0;

      subscriber.Subscribe<Message>(m => notified1 = ++notificationCounter);
      subscriber.Subscribe<Message>(m => notified2 = ++notificationCounter);

      target.Publish(Message.Empty);

#if !SILVERLIGHT
      DisplayContext.Current.Dispatcher.DoEvents();
#endif

      Assert.AreEqual(1, notified1);
      Assert.AreEqual(2, notified2);
    }

    [TestMethod]
    public void StaticCallbackMethodIsInvoked()
    {
      Messenger target = new Messenger();

      _wasStaticCallbackMethodInvoked = false;

      target.Subscribe<Message>(StaticCallbackMethod);
      target.Publish(Message.Empty);

      Assert.IsTrue(_wasStaticCallbackMethodInvoked);
    }

    public static bool _wasStaticCallbackMethodInvoked;

    public static void StaticCallbackMethod(Message m)
    {
      _wasStaticCallbackMethodInvoked = true;
    }
    
    public class TestMessage
    {
      public string Name { get; set; }
    }

    public class UnregisterMessageSubscriberTester
    {
      private readonly MessageSubscriber _subscriber;

      public UnregisterMessageSubscriberTester(Messenger messenger)
      {
        _subscriber = new MessageSubscriber(messenger);
      }

      public bool Received { get; set; }

      public void Action(Message testMessage)
      {
        Received = true;
      }

      public void Register()
      {
        _subscriber.Subscribe<Message>(Action);
      }

      public void Unregister()
      {
        _subscriber.Unsubscribe();
      }
    }

    public class UnregisterTokenTester
    {
      private readonly Messenger _messenger;
      private SubscriptionToken _token1;
      private SubscriptionToken _token2;

      public UnregisterTokenTester(Messenger messenger)
      {
        _messenger = messenger;
      }

      public bool Received1 { get; set; }
      public bool Received2 { get; set; }

      public void Action1(Message testMessage)
      {
        Received1 = true;
      }
      public void Action2(Message testMessage)
      {
        Received2 = true;
      }

      public void Register()
      {
        _token1 = _messenger.Subscribe<Message>(Action1);
        _token2 = _messenger.Subscribe<Message>(Action2);
      }

      public void Unregister1()
      {
        _messenger.Unsubscribe(_token1);
      }
      public void Unregister2()
      {
        _messenger.Unsubscribe(_token2);
      }

    }

    public class DisposeTester
    {
      private readonly Messenger _messenger;

      public DisposeTester(Messenger messenger)
      {
        _messenger = messenger;
      }

      public bool Received { get; set; }

      public void Action(Message testMessage)
      {
        Received = true;
      }

      public void Register()
      {
        _messenger.Subscribe<Message>(Action);
      }

      public void Unregister()
      {
        //_messenger.Unsubscribe(this);
      }
    }

    public class DisposeAnonymousTester
    {
      private readonly Messenger _messenger;

      public DisposeAnonymousTester(Messenger messenger)
      {
        _messenger = messenger;
      }

      public bool Received { get; set; }

      public void Register()
      {
        _messenger.Subscribe<Message>(m => Received = true);
      }

      public void Unregister()
      {
        //_messenger.Unsubscribe(this);
      }
    }

  }
}
