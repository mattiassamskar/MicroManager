using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;

[assembly: InternalsVisibleTo("MicroManager.Tests")]

namespace MicroManager
{
  public class MainWindowViewModel : ViewModelBase
  {
    private readonly IServiceHandler _serviceHandler;

    private string _searchString;

    private bool _isEnabled;

    private NotifyIconHandler _notifyIconHandler;

    public MainWindowViewModel(IServiceHandler serviceHandler)
    {
      ServiceInfoViewModels = new ObservableCollection<ServiceInfoViewModel>();
      SearchCommand = new RelayCommand(SearchCommandCanExecute, SearchCommandExecuted);
      StartServicesCommand = new RelayCommand(StartServicesCommandCanExecute, StartServicesCommandExecuted);
      StopServicesCommand = new RelayCommand(StopServicesCommandCanExecute, StopServicesCommandExecuted);
      IsEnabled = true;

      _serviceHandler = serviceHandler;
      _serviceHandler.RegisterEventWatcher();
      _serviceHandler.ServiceInfoObservable.Subscribe(
        service =>
        {
          var myService = ServiceInfoViewModels.SingleOrDefault(s => s.Name == service.Name);
          if (myService == null) return;
          myService.State = service.State;
          ServiceInfosObservable.OnNext(
            ServiceInfoViewModels.Select(s => new ServiceInfo {Name = s.Name, State = s.State, Enabled = s.Enabled})
              .ToList());
        });

      _notifyIconHandler = new NotifyIconHandler(ServiceInfosObservable);
    }

    public ObservableCollection<ServiceInfoViewModel> ServiceInfoViewModels { get; set; }

    public Subject<List<ServiceInfo>> ServiceInfosObservable { get; set; } = new Subject<List<ServiceInfo>>();

    public RelayCommand SearchCommand { get; set; }

    public RelayCommand StartServicesCommand { get; set; }

    public RelayCommand StopServicesCommand { get; set; }

    public string SearchString
    {
      get { return _searchString; }
      set
      {
        _searchString = value;
        OnPropertyChanged();
        SearchCommand.RaiseCanExecuteChanged();
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

    internal void SearchCommandExecuted()
    {
      ServiceInfoViewModels.Clear();
      _serviceHandler.GetServices(SearchString)
        .ToList()
        .ForEach(
          s => ServiceInfoViewModels.Add(new ServiceInfoViewModel(_serviceHandler) { Name = s.Name, State = s.State }));

      ServiceInfosObservable.OnNext(
        ServiceInfoViewModels.Select(s => new ServiceInfo { Name = s.Name, State = s.State, Enabled = s.Enabled })
          .ToList());
    }

    private bool SearchCommandCanExecute()
    {
      return IsEnabled;
    }

    private async void StartServicesCommandExecuted()
    {
      try
      {
        IsEnabled = false;
        await _serviceHandler.StartServicesAsync(EnabledServices());
      }
      catch (Exception exception)
      {
        MessageBox.Show(exception.Message, "MicroManager", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      finally
      {
        IsEnabled = true;
      }
    }

    private bool StartServicesCommandCanExecute()
    {
      return IsEnabled;
    }

    private async void StopServicesCommandExecuted()
    {
      try
      {
        IsEnabled = false;
        await _serviceHandler.StopServicesAsync(EnabledServices());
      }
      catch (Exception exception)
      {
        MessageBox.Show(exception.Message, "MicroManager", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      finally
      {
        IsEnabled = true;
      }
    }

    private bool StopServicesCommandCanExecute()
    {
      return IsEnabled;
    }

    private List<string> EnabledServices()
    {
      return ServiceInfoViewModels.Where(si => si.Enabled).Select(s => s.Name).ToList();
    }
  }
}