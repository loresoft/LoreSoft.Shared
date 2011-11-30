using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using LoreSoft.Shared.Messaging;

namespace LoreSoft.Shared.Controls
{
  public class Window : System.Windows.Window, IWindow
  {
    public Window()
    {
      Loaded += OnWindowLoaded;
    }
    
    public Window(object parameter)
      : this()
    {
      Tag = parameter;
    }

    protected virtual void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
      Loaded -= OnWindowLoaded;

      var viewModel = DataContext as IWindowViewModel;
      if (viewModel == null)
        return;

      viewModel.WindowOpened(this);
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);

      var viewModel = DataContext as IWindowViewModel;
      if (viewModel == null)
        return;

      viewModel.WindowClosed(DialogResult);
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
      base.OnClosing(e);

      var viewModel = DataContext as IWindowViewModel;
      if (viewModel == null)
        return;

      viewModel.WindowClosing(e);
    }

    public void CloseView()
    {
      CloseView(null, Tag);

    }

    public void CloseView(object parameter)
    {
      CloseView(null, parameter);
    }

    public void CloseView(bool? dialogResult, object parameter)
    {
      Tag = parameter;

      DialogResult = dialogResult;
      Close();

      var message = new WindowMessage(this)
      {
        Action = WindowAction.Close,
        WindowName = Name,
        Parameter = parameter,
        DialogResult = dialogResult
      };
      Messenger.Current.PublishAsync(message);
    }

    public void ShowView()
    {

    }

    public void ShowView(object parameter)
    {
      Tag = parameter;
      Show();

      var message = new WindowMessage(this)
      {
        Action = WindowAction.Open,
        WindowName = Name,
        Parameter = parameter
      };
      Messenger.Current.PublishAsync(message);
    }

    public new object Title
    {
      get { return base.Title; }
      set { base.Title = value == null ? string.Empty : value.ToString(); }
    }
  }
}
