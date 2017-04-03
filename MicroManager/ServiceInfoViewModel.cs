using System.Windows.Media;

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
      StartStopToggleCommand = new RelayCommand(StartStopToggleCommandCanExecute, StartStopToggleCommandExecuted);
      _enabled = true;
    }

    public RelayCommand StartStopToggleCommand { get; set; }

    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        OnPropertyChanged();
      }
    }

    public string State
    {
      get { return _state; }
      set
      {
        _state = value;
        OnPropertyChanged();
        OnPropertyChanged("Background");
      }
    }

    public bool Enabled
    {
      get { return _enabled; }
      set
      {
        _enabled = value;
        OnPropertyChanged();
      }
    }

    public SolidColorBrush Background => State == "Running" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);

    internal void StartCommandExecuted()
    {
      _serviceHandler.StartService(_name);
    }

    internal void StopCommandExecuted()
    {
      _serviceHandler.StopService(_name);
    }

    internal void StartStopToggleCommandExecuted()
    {
      switch (State)
      {
        case "Running":
          StopCommandExecuted();
          break;
        case "Stopped":
          StartCommandExecuted();
          break;
      }
    }

    private bool StartStopToggleCommandCanExecute()
    {
      return State == "Running" || State == "Stopped";
    }
  }
}