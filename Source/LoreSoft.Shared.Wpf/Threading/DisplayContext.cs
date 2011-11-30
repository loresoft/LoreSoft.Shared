using System;
using System.Windows;
using System.Windows.Threading;
using LoreSoft.Shared.ComponentModel;

namespace LoreSoft.Shared.Threading
{
  public class DisplayContext : SingletonBase<DisplayContext>
  {
    public DisplayContext()
    {
#if SILVERLIGHT
      _dispatcher = Deployment.Current.Dispatcher;
#else
      _dispatcher = Dispatcher.CurrentDispatcher;
#endif
    }

    public void Initialize()
    { }

    private readonly Dispatcher _dispatcher;
    public Dispatcher Dispatcher
    {
      get { return _dispatcher; }
    }
  }
}
