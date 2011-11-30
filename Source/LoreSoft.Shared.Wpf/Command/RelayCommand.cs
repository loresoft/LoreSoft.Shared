using System;
using System.Diagnostics;
using System.Windows.Input;

namespace LoreSoft.Shared.Command
{
  /// <summary>
  /// An <see cref="ICommand"/> implementation that relays the executing of the command to a delegate.
  /// </summary>
  public class RelayCommand : ICommand
  {
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    /// <summary>
    /// Initializes a new instance of the RelayCommand class that can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
    public RelayCommand(Action execute)
      : this(execute, null)
    { }

    /// <summary>
    /// Initializes a new instance of the RelayCommand class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
    public RelayCommand(Action execute, Func<bool> canExecute)
    {
      if (execute == null)
        throw new ArgumentNullException("execute");

      _execute = execute;
      _canExecute = canExecute;
    }

    protected RelayCommand()
    {
      
    }

    /// <summary>
    /// Occurs when changes occur that affect whether the command should execute.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged" /> event.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
      var handler = CanExecuteChanged;
      if (handler == null)
        return;

      handler(this, EventArgs.Empty);
    }

    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">This parameter will always be ignored.</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    [DebuggerStepThrough]
    public virtual bool CanExecute(object parameter)
    {
      return _canExecute == null ? true : _canExecute();
    }

    /// <summary>
    /// Defines the method to be called when the command is invoked. 
    /// </summary>
    /// <param name="parameter">This parameter will always be ignored.</param>
    public virtual void Execute(object parameter)
    {
      _execute();
    }
  } 
}
