using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace LoreSoft.Shared.Messaging
{
  /// <summary>
  /// The class holds a list of subscriptions that are acting as proxies for delegates.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This class is part of the work around for Silverlight only being able to call an accessible method from a delegate.
  /// It works but wrapping the delegate with MessageAction and then subscribing that with the messenger.
  /// </para>
  /// <para>
  /// The code must have a reference to the MessageSubscriber.  Messenger is holding a WeakReference to subscriptions, 
  /// if you don’t have a reference in the class, the target of the delegates could be null and you would not receive the message.
  /// </para>
  /// </remarks>
  public class MessageSubscriber
  {
    private readonly List<object> _actions = new List<object>();
    private readonly List<SubscriptionToken> _tokens = new List<SubscriptionToken>();
    private readonly IMessenger _messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageSubscriber"/> class using the default messenger.
    /// </summary>
    public MessageSubscriber()
      : this(Messenger.Current)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageSubscriber"/> class using the specified messenger.
    /// </summary>
    /// <param name="messenger">The messenger to use when subscribing.</param>
    public MessageSubscriber(IMessenger messenger)
    {
      _messenger = messenger;
    }

    /// <summary>
    /// Subscribes the specified action for message of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to subscribe to.</typeparam>
    /// <param name="action">The action to run when a message of type <typeparamref name="TMessage"/> is published.</param>
    public void Subscribe<TMessage>(Action<TMessage> action)
    {
#if SILVERLIGHT
      var dispatcher = Deployment.Current.Dispatcher;
#else
      var dispatcher = Dispatcher.CurrentDispatcher;
#endif
      Subscribe<TMessage>(action, dispatcher);
    }

    /// <summary>
    /// Subscribes the specified action for message of type <typeparamref name="TMessage"/>.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message to subscribe to.</typeparam>
    /// <param name="action">The action to run when a message of type <typeparamref name="TMessage"/> is published.</param>
    /// <param name="dispatcher">The dispatcher.</param>
    public void Subscribe<TMessage>(Action<TMessage> action, Dispatcher dispatcher)
    {
      var messageAction = new MessageAction<TMessage>(action, dispatcher);
      _actions.Add(messageAction);

      var token = _messenger.Subscribe<TMessage>(messageAction.Invoke);
      _tokens.Add(token);
    }

    /// <summary>
    /// Unsubscribes all subscriptions that this <see cref="MessageSubscriber"/> created.
    /// </summary>
    public void Unsubscribe()
    {
      _tokens.ForEach(t => _messenger.Unsubscribe(t));

      _tokens.Clear();
      _actions.Clear();
    }
  }
}