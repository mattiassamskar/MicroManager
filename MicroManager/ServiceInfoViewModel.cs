using System;
using System.Reactive.Linq;
using System.Windows.Media;

namespace MicroManager
{
  using System.Threading.Tasks;

  public class ServiceInfoViewModel : ViewModelBase
  {
    private readonly IServiceHandler _serviceHandler;

    private bool _isEnabled = true;

    private bool _included = true;

    public ServiceInfo _serviceInfo;

    private string _message;

    public ServiceInfoViewModel(IServiceHandler serviceHandler, ServiceInfo serviceInfo)
    {
      _serviceHandler = serviceHandler;
      _serviceInfo = serviceInfo;
      StartStopToggleCommand = new RelayCommand(() => IsEnabled, StartStopToggleCommandExecuted);

      _serviceHandler.ServiceInfosObservable.Where(s => s.Name == Name).Subscribe(s => State = s.State);
    }

    public RelayCommand StartStopToggleCommand { get; set; }

    public string Name
    {
      get { return _serviceInfo.Name; }
      set
      {
        _serviceInfo.Name = value;
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
      get { return _serviceInfo.State; }
      set
      {
        _serviceInfo.State = value;
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

    internal async Task StartCommandExecuted()
    {
      try
      {
        IsEnabled = false;
        await _serviceHandler.StartServiceAsync(_serviceInfo.Name);
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

    internal async Task StopCommandExecuted()
    {
      try
      {
        IsEnabled = false;
        await _serviceHandler.StopServiceAsync(_serviceInfo.Name);
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

    internal async void StartStopToggleCommandExecuted()
    {
      switch (State)
      {
        case "Running":
          await StopCommandExecuted();
          break;
        case "Stopped":
          await StartCommandExecuted();
          break;
      }
    }

    public class ServiceInfo
    {
      public string Name { get; set; }

      public string State { get; set; }

      public bool Included { get; set; }
    }
  }
}