using System;
using System.Windows.Threading;

namespace LoreSoft.Shared.Messaging
{
  /// <summary>
  /// A wrapper class to work around Silverlight only being able to call an accessible method from a delegate.
  /// </summary>
  /// <typeparam name="T">The type of the parameter of the method that this action encapsulates.</typeparam>
  /// <remarks>
  /// <para>
  /// This class is a work around for Silverlight requirement that a delegate be an accessible method. The class acts
  ///  as a proxy between the actual delegate and the one that gets subscribed to in Messenger. 
  /// </para>
  /// </remarks>
  public class MessageAction<T>
  {
    private readonly Action<T> _action;
    private readonly Dispatcher _dispatcher; 
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageAction&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    public MessageAction(Action<T> action)
      :this(action, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageAction&lt;T&gt;"/> class.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="dispatcher">The dispatcher.</param>
    public MessageAction(Action<T> action, Dispatcher dispatcher)
    {
      _action = action;
      _dispatcher = dispatcher;
    }

    /// <summary>
    /// Invokes the specified action.
    /// </summary>
    /// <param name="arg">The argument to pass to the action.</param>
    public void Invoke(T arg)
    {
      if (_dispatcher != null)
        _dispatcher.BeginInvoke(_action, arg);
      else
        _action(arg);
    }
  }
}