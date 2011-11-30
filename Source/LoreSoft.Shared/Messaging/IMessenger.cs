using System;

namespace LoreSoft.Shared.Messaging
{
    /// <summary>
    /// An interface defining the messenger system.
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Subscribes the specified action for message of type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to subscribe to.</typeparam>
        /// <param name="action">The action to run when a message of type <typeparamref name="TMessage"/> is published.</param>
        SubscriptionToken Subscribe<TMessage>(Action<TMessage> action);

        /// <summary>
        /// Publishes the specified message to all the subscribers to message type <typeparamref name="TMessage"/>.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        /// <param name="message">The message to send to the subscribers.</param>
        /// <returns>The number of subscribers notified.</returns>
        int Publish<TMessage>(TMessage message);

        /// <summary>
        /// Publishes the specified message to all the subscribers to message type <typeparamref name="TMessage"/>
        /// without blocking the calling thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to publish.</typeparam>
        /// <param name="message">The message to send to the subscribers.</param>
        void PublishAsync<TMessage>(TMessage message);

        /// <summary>
        /// Unsubscribes the action identified by the <see cref="SubscriptionToken"/>.
        /// </summary>
        /// <param name="token">The subscription token to remove.</param>
        void Unsubscribe(SubscriptionToken token);
    }
}