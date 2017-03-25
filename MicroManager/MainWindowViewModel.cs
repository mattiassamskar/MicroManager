using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MicroManager.Tests")]

namespace MicroManager
{
  public class MainWindowViewModel : ViewModelBase
  {
    private readonly IServiceHandler _serviceHandler;

    private string _searchString;

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
        });
    }

    public ObservableCollection<ServiceInfoViewModel> ServiceInfoViewModels { get; set; }

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
      if (string.IsNullOrEmpty(SearchString)) return;

      ServiceInfoViewModels.Clear();
      _serviceHandler.GetServices(SearchString)
        .ToList()
        .ForEach(
          s => ServiceInfoViewModels.Add(new ServiceInfoViewModel(_serviceHandler) {Name = s.Name, State = s.State}));
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