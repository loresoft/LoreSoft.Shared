using System;

namespace LoreSoft.Shared.Messaging
{
    /// <summary>
    /// A message for when a property changed.
    /// </summary>
    public class PropertyChangedMessage : Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="propertyName">Name of the property.</param>
        public PropertyChangedMessage(object sender, string propertyName)
            : base(sender)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public PropertyChangedMessage(object sender, string propertyName, object oldValue, object newValue)
            : base(sender)
        {
            PropertyName = propertyName;
            NewValue = newValue;
            OldValue = oldValue;
        }

        /// <summary>
        /// Gets or sets the name of the property that changed.
        /// </summary>
        public string PropertyName { get; protected set; }

        /// <summary>
        /// Gets the value that the property has after the change.
        /// </summary>
        public object NewValue { get; private set; }

        /// <summary>
        /// Gets the value that the property had before the change.
        /// </summary>
        public object OldValue { get; private set; }

    }
}
