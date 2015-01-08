using System;
using System.Windows.Input;

namespace LdapSearch
{
  public class RelayCommand : ICommand
  {
    public RelayCommand(Func<bool> canExecute, Action execute)
    {
      this.canExecute = canExecute;
      this.execute = execute;
    }

    private readonly Func<bool> canExecute;
    private readonly Action execute;

    public bool CanExecute(object parameter)
    {
      return canExecute();
    }

    public void Execute(object parameter)
    {
      execute();
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
      if (CanExecuteChanged != null)
        CanExecuteChanged(this, EventArgs.Empty);
    }
  }
}