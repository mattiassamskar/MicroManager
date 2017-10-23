using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MicroManager
{
  public class ServiceInfoViewModel : ViewModelBase
  {
    private readonly IServiceHandler _serviceHandler;

    private string _name;
    private string _state;
    private bool _isEnabled;
    private bool _isIncluded;
    private string _toolTip;

    public ServiceInfoViewModel(IServiceHandler serviceHandler, ServiceInfo serviceInfo)
    {
      _serviceHandler = serviceHandler;
      Name = serviceInfo.Name;
      State = serviceInfo.State;
      IsEnabled = true;
      IsIncluded = true;
      ToggleCommand = new RelayCommand(() => IsEnabled, ToggleCommandExecuted);
      ToggleIsIncludedCommand = new RelayCommand(() => IsEnabled, ToggleIsIncludedCommandExecuted);

      _serviceHandler.ServiceInfosObservable.Where(s => s.Name == Name).Subscribe(s => State = s.State);
    }

    public RelayCommand ToggleCommand { get; set; }

    public RelayCommand ToggleIsIncludedCommand { get; set; }

    public string Name
    {
      get => _name;
      set
      {
        _name = value;
        OnPropertyChanged();
      }
    }

    public string ToolTip
    {
      get => _toolTip;
      private set
      {
        _toolTip = value;
        OnPropertyChanged();
      }
    }

    public string State
    {
      get => _state;
      set
      {
        if (_state == value) return;

        _state = value;
        ToolTip = State;
        OnPropertyChanged();
        OnPropertyChanged("Background");
      }
    }

    public bool IsIncluded
    {
      get => _isIncluded;
      set
      {
        _isIncluded = value;
        OnPropertyChanged();
        OnPropertyChanged("Background");
      }
    }

    public bool IsEnabled
    {
      get => _isEnabled;
      set
      {
        _isEnabled = value;
        OnPropertyChanged();
      }
    }

    public SolidColorBrush Background
    {
      get
      {
        if (!IsIncluded)
          return new SolidColorBrush(Colors.Gray);

        switch (State)
        {
          case "Running":
            return new SolidColorBrush(Color.FromRgb(34, 177, 76));
          case "Stopped":
            return new SolidColorBrush(Color.FromRgb(237, 28, 36));
          default:
            return new SolidColorBrush(Colors.Yellow);
        }
      }
    }

    public async Task StartServiceAsync()
    {
      await RunAsync(() => _serviceHandler.StartService(Name));
    }

    public async Task StopServiceAsync()
    {
      await RunAsync(() => _serviceHandler.StopService(Name));
    }

    public async void ToggleCommandExecuted()
    {
      await RunAsync(() => _serviceHandler.ToggleService(Name));
    }

    public void ToggleIsIncludedCommandExecuted()
    {
      IsIncluded = !IsIncluded;
    }

    private async Task RunAsync(Action action)
    {
      try
      {
        IsEnabled = false;
        await Task.Run(action);
      }
      catch (Exception exception)
      {
        ToolTip = exception.Message + " " + exception.InnerException?.Message;
      }
      finally
      {
        IsEnabled = true;
      }
    }
  }
}