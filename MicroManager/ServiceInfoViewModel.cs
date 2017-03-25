namespace MicroManager
{
  public class ServiceInfoViewModel : ViewModelBase
  {
    private readonly IServiceHandler _serviceHandler;

    private string _name;

    private string _state;

    private bool _enabled;

    public ServiceInfoViewModel(IServiceHandler serviceHandler)
    {
      _serviceHandler = serviceHandler;
      StartServiceCommand = new RelayCommand(StartCommandCanExecute, StartCommandExecuted);
      StopServiceCommand = new RelayCommand(StopCommandCanExecute, StopCommandExecuted);
      _enabled = true;
    }

    public RelayCommand StartServiceCommand { get; set; }

    public RelayCommand StopServiceCommand { get; set; }

    public string Name
    {
      get { return _name; }
      set
      {
        OnPropertyChanged();
        _name = value;
      }
    }

    public string State
    {
      get { return _state; }
      set
      {
        OnPropertyChanged();
        _state = value;
      }
    }

    public bool Enabled
    {
      get { return _enabled; }
      set
      {
        OnPropertyChanged();
        _enabled = value;
      }
    }

    internal void StartCommandExecuted()
    {
      _serviceHandler.StartService(_name);
    }

    private bool StartCommandCanExecute()
    {
      return true;
    }

    internal void StopCommandExecuted()
    {
      _serviceHandler.StopService(_name);
    }

    private bool StopCommandCanExecute()
    {
      return true;
    }
  }
}