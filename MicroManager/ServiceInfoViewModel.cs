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
    private bool _included;
    private string _message;

    public ServiceInfoViewModel(IServiceHandler serviceHandler, ServiceInfo serviceInfo)
    {
      _serviceHandler = serviceHandler;
      Name = serviceInfo.Name;
      State = serviceInfo.State;
      IsEnabled = true;
      Included = true;
      ToggleCommand = new RelayCommand(() => IsEnabled, ToggleCommandExecuted);

      _serviceHandler.ServiceInfosObservable.Where(s => s.Name == Name).Subscribe(s => State = s.State);
    }

    public RelayCommand ToggleCommand { get; set; }

    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
        OnPropertyChanged();
      }
    }

    public string Message
    {
      get
      {
        return _message;
      }
      private set
      {
        _message = value;
        OnPropertyChanged();
      }
    }

    public string State
    {
      get { return _state; }
      set
      {
        if (_state == value) return;

        _state = value;
        Message = string.Empty;
        OnPropertyChanged();
        OnPropertyChanged("Background");
      }
    }

    public bool Included
    {
      get { return _included; }
      set
      {
        _included = value;
        OnPropertyChanged();
      }
    }

    public bool IsEnabled
    {
      get
      {
        return _isEnabled;
      }
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

    private async Task RunAsync(Action action)
    {
      try
      {
        IsEnabled = false;
        await Task.Run(action);
      }
      catch (Exception exception)
      {
        Message = exception.Message + " " + exception.InnerException?.Message;
      }
      finally
      {
        IsEnabled = true;
      }
    }
  }
}