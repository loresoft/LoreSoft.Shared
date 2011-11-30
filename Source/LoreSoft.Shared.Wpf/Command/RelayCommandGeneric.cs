using System;
using System.Windows.Input;

namespace LoreSoft.Shared.Command
{
  /// <summary>
  /// An <see cref="ICommand"/> implementation that relays the executing of the command to a delegate.
  /// </summary>
  /// <typeparam name="TParameter">The type of the parameter passed to the relay delegate.</typeparam>
  public class RelayCommand<TParameter> : RelayCommand
  {
    private readonly Action<TParameter> _execute;
    private readonly Predicate<TParameter> _canExecute;

    /// <summary>
    /// Initializes a new instance of the RelayCommand class that 
    /// can always execute.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
    public RelayCommand(Action<TParameter> execute)
      : this(execute, null)
    { }

    /// <summary>
    /// Initializes a new instance of the RelayCommand class.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    /// <exception cref="ArgumentNullException">If the execute argument is null.</exception>
    public RelayCommand(Action<TParameter> execute, Predicate<TParameter> canExecute)
    {
      if (execute == null)
        throw new ArgumentNullException("execute");

      _execute = execute;
      _canExecute = canExecute;
    }
    
    /// <summary>
    /// Defines the method that determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data 
    /// to be passed, this object can be set to a null reference</param>
    /// <returns>true if this command can be executed; otherwise, false.</returns>
    public override bool CanExecute(object parameter)
    {
      return _canExecute == null ? true : _canExecute((TParameter)parameter);
    }

    /// <summary>
    /// Defines the method to be called when the command is invoked. 
    /// </summary>
    /// <param name="parameter">Data used by the command. If the command does not require data 
    /// to be passed, this object can be set to a null reference</param>
    public override void Execute(object parameter)
    {
      _execute((TParameter)parameter);
    }
  }
}
