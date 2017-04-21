using System;
using System.Windows;
using System.Windows.Media;

namespace MicroManager
{
  public class ServiceInfoViewModel : ViewModelBase
  {
    private readonly IServiceHandler _serviceHandler;

    private bool _isEnabled = true;

    private bool _included = true;

    public ServiceInfo _serviceInfo => new ServiceInfo();

    public ServiceInfoViewModel(IServiceHandler serviceHandler)
    {
      _serviceHandler = serviceHandler;
      StartStopToggleCommand = new RelayCommand(() => IsEnabled, StartStopToggleCommandExecuted);
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

    public string State
    {
      get { return _serviceInfo.State; }
      set
      {
        _serviceInfo.State = value;
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
          default:
            return new SolidColorBrush(Color.FromRgb(237, 28, 36));
        }
      }
    }

    internal async void StartCommandExecuted()
    {
      try
      {
        IsEnabled = false;
        await _serviceHandler.StartServiceAsync(_serviceInfo.Name);
      }
      catch (Exception exception)
      {
        var message = exception.Message + " " + exception.InnerException?.Message;
        MessageBox.Show(message, "MicroManager", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      finally
      {
        IsEnabled = true;
      }
    }

    internal async void StopCommandExecuted()
    {
      try
      {
        IsEnabled = false;
        await _serviceHandler.StopServiceAsync(_serviceInfo.Name);
      }
      catch (Exception exception)
      {
        var message = exception.Message + " " + exception.InnerException?.Message;
        MessageBox.Show(message, "MicroManager", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      finally
      {
        IsEnabled = true;
      }
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

    public class ServiceInfo
    {
      public string Name { get; set; }

      public string State { get; set; }

      public bool Enabled { get; set; }
    }
  }
}