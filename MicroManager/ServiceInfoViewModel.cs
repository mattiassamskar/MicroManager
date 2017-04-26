using System;
using System.Reactive.Linq;
using System.Windows.Media;

namespace MicroManager
{
  using System.Threading.Tasks;

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
      StartStopToggleCommand = new RelayCommand(() => IsEnabled, StartStopToggleCommandExecuted);

      _serviceHandler.ServiceInfosObservable.Where(s => s.Name == Name).Subscribe(s => State = s.State);
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
      try
      {
        IsEnabled = false;
        await Task.Run(() => _serviceHandler.StartService(Name));
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

    public async Task StopServiceAsync()
    {
      try
      {
        IsEnabled = false;
        await Task.Run(() => _serviceHandler.StopService(Name));
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

    public async void StartStopToggleCommandExecuted()
    {
      switch (State)
      {
        case "Running":
          await StopServiceAsync();
          break;
        case "Stopped":
          await StartServiceAsync();
          break;
      }
    }
  }
}