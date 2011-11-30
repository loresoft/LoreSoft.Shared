using System;
using System.Windows;

namespace LoreSoft.Shared.Messaging
{
  /// <summary>
  /// A message to display a dialog box.
  /// </summary>
  public class DialogMessage : Message
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogMessage"/> class.
    /// </summary>
    public DialogMessage()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogMessage"/> class.
    /// </summary>
    /// <param name="sender">The sender.</param>
    public DialogMessage(object sender)
      : base(sender)
    { }

    /// <summary>
    /// Gets or sets the MessageBoxButton value that specifies which button or buttons to display.
    /// </summary>
    public MessageBoxButton Button { get; set; }

    /// <summary>
    /// Gets a callback method that should be executed to deliver the result
    /// of the message box to the object that sent the message.
    /// </summary>
    public Action<MessageBoxResult> CompleteCallback { get; set; }

    /// <summary>
    /// Gets or sets a String that specifies the title bar caption to display.
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// Gets or sets the String that specifies the text to display.
    /// </summary>
    public string Content { get; set; }

#if !SILVERLIGHT
    /// <summary>
    /// Gets or sets the MessageBoxImage value that specifies the icon to display.
    /// </summary>
    public MessageBoxImage Icon { get; set; }

    /// <summary>
    /// Gets or sets the MessageBoxOptions value object that specifies the options.
    /// </summary>
    public MessageBoxOptions Options { get; set; }
#endif

    /// <summary>
    /// Called when the dialog completes.
    /// </summary>
    /// <param name="result">The result of the message box.</param>
    public void DialogComplete(MessageBoxResult result)
    {
      if (CompleteCallback == null)
        return;

      CompleteCallback(result);
    }
  }
}
