using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using LoreSoft.Shared.Extensions;

namespace LoreSoft.Shared.Messaging
{
    /// <summary>
    /// A Messenger class enables communications between loosely coupled components in the application. 
    /// Messenger works by allowing publishers and subscribers to communicate through messages while not
    /// having a direct reference to each other.
    /// </summary>
    public class Messenger : IMessenger
    {
        private readonly List<Subscription> _subscriptions = new List<Subscription>();
        private readonly object _lockObject = new object();

        /// <summary>
        /// Subscribes the specified action for message of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to subscribe to.</typeparam>
        /// <param name="action">The action to run when a message of type <typeparamref name="TMessage"/> is published.</param>
        public SubscriptionToken Subscribe<TMessage>(Action<TMessage> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Type messageType = typeof(TMessage);
            var subscription = new Subscription(action, messageType);

            lock (_lockObject)
                _subscriptions.Add(subscription);

            return subscription.Token;
        }

        /// <summary>
        /// Publishes the specified message to all the subscribers to message type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        /// <param name="message">The message to send to the subscribers.</param>
        /// <returns>The number of subscribers notified.</returns>
        public int Publish<TMessage>(TMessage message)
        {
            if (null == message)
                throw new ArgumentNullException("message");

            int count = 0;
            Type messageType = typeof(TMessage);
            string messageName = messageType.AssemblyQualifiedName;

            List<Subscription> subscribers;
            lock (_lockObject)
            {
                subscribers = _subscriptions
                  .Where(s => s.MessageKey == messageName)
                  .ToList();
            }

            foreach (var subscription in subscribers)
            {
                var action = subscription.CreateDelegate();
                if (action == null)
                {
                    // target no longer active, remove subscription
                    lock (_lockObject)
                        _subscriptions.Remove(subscription);

                    continue;
                }

                count++;
                try
                {
                    action.DynamicInvoke(message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error Sending Message:" + ex.Message);
                }
            }

            return count;
        }

        /// <summary>
        /// Publishes the specified message to all the subscribers to message type <typeparamref name="TMessage"/>
        /// without blocking the calling thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        /// <param name="message">The message to send to the subscribers.</param>
        public void PublishAsync<TMessage>(TMessage message)
        {
            ThreadPool.QueueUserWorkItem(m => Publish((TMessage)m), message);
        }

        /// <summary>
        /// Unsubscribes the action identified by the <see cref="SubscriptionToken"/>.
        /// </summary>
        /// <param name="token">The subscription token to remove.</param>
        public void Unsubscribe(SubscriptionToken token)
        {
            lock (_lockObject)
                _subscriptions.RemoveAll(s => s.Token == token);
        }

        #region Singleton
        private static readonly Lazy<Messenger> _current = new Lazy<Messenger>(() => new Messenger());

        /// <summary>
        /// Gets the singleton instance of <see cref="Messenger"/>.
        /// </summary>
        public static Messenger Current
        {
            get { return _current.Value; }
        }
        #endregion
    }
}
