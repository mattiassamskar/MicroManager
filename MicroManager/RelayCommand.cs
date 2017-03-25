using System;
using System.Windows.Input;

namespace MicroManager
{
  public class RelayCommand : ICommand
  {
    public RelayCommand(Func<bool> canExecute, Action execute)
    {
      _canExecute = canExecute;
      _execute = execute;
    }

    private readonly Func<bool> _canExecute;
    private readonly Action _execute;

    public bool CanExecute(object parameter)
    {
      return _canExecute();
    }

    public void Execute(object parameter)
    {
      _execute();
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}