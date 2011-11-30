using System.ComponentModel;

namespace LoreSoft.Shared.Controls
{
  public interface IWindowViewModel
  {
    void WindowOpened(IWindow window);
    void WindowClosing(CancelEventArgs e);
    void WindowClosed(bool? result);
  }
}