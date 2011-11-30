using System.Windows.Threading;

namespace LoreSoft.Shared.Controls
{
  public interface IWindow
  {
    void CloseView();
    void CloseView(object parameter);
    void CloseView(bool? dialogResult, object parameter);
    
    void ShowView();
    void ShowView(object parameter);

    Dispatcher Dispatcher { get;  }
    string Name { get; }
    object Title { get; set; }
    object Tag { get; set; }
  }
}