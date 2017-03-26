using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MicroManager.Tests")]

namespace MicroManager
{
  public class MainWindowViewModel : ViewModelBase
  {
    private readonly IServiceHandler _serviceHandler;

    private string _searchString;

    private NotifyIconHandler _notifyIconHandler;

    public MainWindowViewModel(IServiceHandler serviceHandler)
    {
      ServiceInfoViewModels = new ObservableCollection<ServiceInfoViewModel>();
      SearchCommand = new RelayCommand(SearchCommandCanExecute, SearchCommandExecuted);
      StartServicesCommand = new RelayCommand(StartServicesCommandCanExecute, StartServicesCommandExecuted);
      StopServicesCommand = new RelayCommand(StopServicesCommandCanExecute, StopServicesCommandExecuted);

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
      return true;
    }

    private void StartServicesCommandExecuted()
    {
      _serviceHandler.StartServices(EnabledServices());
    }

    private bool StartServicesCommandCanExecute()
    {
      return true;
    }

    private void StopServicesCommandExecuted()
    {
      _serviceHandler.StopServices(EnabledServices());
    }

    private bool StopServicesCommandCanExecute()
    {
      return true;
    }

    private List<string> EnabledServices()
    {
      return ServiceInfoViewModels.Where(si => si.Enabled).Select(s => s.Name).ToList();
    }
  }
}