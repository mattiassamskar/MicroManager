using System.Windows.Media;

namespace MicroManager
{
  public class ServiceInfoViewModel : ViewModelBase
  {
    private readonly IServiceHandler serviceHandler;

    private string name;

    private string state;

    private bool included;

    private bool isEnabled;

    public ServiceInfoViewModel(IServiceHandler serviceHandler)
    {
      this.serviceHandler = serviceHandler;
      StartStopToggleCommand = new RelayCommand(StartStopToggleCommandCanExecute, StartStopToggleCommandExecuted);
      included = true;
      IsEnabled = true;
    }

    public RelayCommand StartStopToggleCommand { get; set; }

    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        OnPropertyChanged();
      }
    }

    public string State
    {
      get { return state; }
      set
      {
        state = value;
        OnPropertyChanged();
        OnPropertyChanged("Background");
      }
    }

    public bool Included
    {
      get { return included; }
      set
      {
        included = value;
        OnPropertyChanged();
      }
    }

    public bool IsEnabled
    {
      get
      {
        return isEnabled;
      }
      set
      {
        isEnabled = value;
        OnPropertyChanged();
      }
    }

    public SolidColorBrush Background => State == "Running" ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);

    internal async void StartCommandExecuted()
    {
      IsEnabled = false;
      await serviceHandler.StartServiceAsync(name);
      IsEnabled = true;
    }

    internal async void StopCommandExecuted()
    {
      IsEnabled = false;
      await serviceHandler.StopServiceAsync(name);
      IsEnabled = true;
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
      return IsEnabled;
    }
  }
}