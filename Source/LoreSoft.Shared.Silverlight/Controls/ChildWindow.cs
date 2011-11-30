using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LoreSoft.Shared.Messaging;
using LoreSoft.Shared.Navigation;

namespace LoreSoft.Shared.Controls
{
  public interface IChildWindow
  {
    void CloseView();
    void CloseView(object parameter);
    void CloseView(bool? dialogResult, object parameter);
    
    void ShowView();
    void ShowView(object parameter);

    Dispatcher Dispatcher { get;  }
    string Name { get; }
    object Title { get; set; }
  }

  public interface IChildWindowViewModel
  {
    void WindowOpened(IChildWindow window);
    void WindowClosing(CancelEventArgs e);
    void WindowClosed(bool? result);
  }


  public class ChildWindow : System.Windows.Controls.ChildWindow, IChildWindow
  {
    public ChildWindow()
    {
      DefaultStyleKey = typeof(System.Windows.Controls.ChildWindow);
    }

    protected override void OnOpened()
    {
      base.OnOpened();

      var viewModel = DataContext as IChildWindowViewModel;
      if (viewModel == null)
        return;

      viewModel.WindowOpened(this);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      var viewModel = DataContext as IChildWindowViewModel;
      if (viewModel == null)
        return;

      viewModel.WindowClosing(e);
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);

      if (Application.Current == null || Application.Current.RootVisual == null)
        return;

      // bug fix
      Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);

      var viewModel = DataContext as IChildWindowViewModel;
      if (viewModel == null)
        return;

      viewModel.WindowClosed(DialogResult);
    }

    public void CloseView()
    {
      CloseView(null, null);
    }

    public void CloseView(object parameter)
    {
      CloseView(null, parameter);
    }

    public void CloseView(bool? dialogResult, object parameter)
    {
      if (DialogResult != dialogResult)
        DialogResult = dialogResult;
      else
        Close();

      var message = new WindowMessage(this)
      {
        Action = WindowAction.Close,
        WindowName = Name,
        Parameter = parameter
      };
      Messenger.Current.PublishAsync(message);
    }

    public void ShowView()
    {
      ShowView(null);
    }

    public void ShowView(object parameter)
    {
      Show();

      var message = new WindowMessage(this)
      {
        Action = WindowAction.Open,
        WindowName = Name,
        Parameter = parameter
      };
      Messenger.Current.PublishAsync(message);
    }
  }


}
