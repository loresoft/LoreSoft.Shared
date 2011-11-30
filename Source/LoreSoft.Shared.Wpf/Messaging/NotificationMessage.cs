using System;
using LoreSoft.Shared.Controls;

namespace LoreSoft.Shared.Messaging
{
  /// <summary>
  /// A notification message.
  /// </summary>
  public class NotificationMessage : Message
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationMessage"/> class.
    /// </summary>
    public NotificationMessage()
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationMessage"/> class.
    /// </summary>
    /// <param name="sender">The sender of the message.</param>
    public NotificationMessage(object sender)
      : base(sender)
    {}

    /// <summary>
    /// Gets or sets the notification.
    /// </summary>
    /// <value>
    /// The notification.
    /// </value>
    public Notification Notification { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to clear existing notfications.
    /// </summary>
    /// <value>
    ///   <c>true</c> to clear existing; otherwise, <c>false</c>.
    /// </value>
    public bool ClearExisting { get; set; }
  }
}
