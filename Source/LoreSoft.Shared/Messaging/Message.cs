using System;

namespace LoreSoft.Shared.Messaging
{
    /// <summary>
    /// A simple message type for <see cref="Messenger"/>
    /// </summary>
    public class Message
    {
        /// <summary>
        /// A default empty message.
        /// </summary>
        public static readonly Message Empty = new Message();

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        public Message()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        public Message(object sender)
        {
            Sender = sender;
        }

        /// <summary>
        /// Gets or sets the sender of the message.
        /// </summary>
        /// <value>
        /// The sender of the messages.
        /// </value>
        public object Sender { get; set; }
    }
}
