using System;

namespace LoreSoft.Shared.Messaging
{
  public enum WindowAction
  {
    Open,
    Close
  }

  public class WindowMessage : Message
  {
    public WindowMessage()
    { }

    public WindowMessage(object sender)
      : base(sender)
    { }

    public string WindowName { get; set; }

    public WindowAction Action { get; set; }

    public object Parameter { get; set; }

    public bool? DialogResult { set; get; }
  }
}
