using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;

[assembly: InternalsVisibleTo("MicroManager.Tests")]

namespace MicroManager
{
  public class MainWindowViewModel : ViewModelBase
  {
    private readonly IServiceHandler serviceHandler;
    private string searchString;
    private bool isEnabled;

    public MainWindowViewModel(IServiceHandler serviceHandler)
    {
      this.serviceHandler = serviceHandler;

      ServiceInfoViewModels = new ObservableCollection<ServiceInfoViewModel>();
      SearchCommand = new RelayCommand(SearchCommandCanExecute, SearchCommandExecuted);
      StartServicesCommand = new RelayCommand(StartServicesCommandCanExecute, StartServicesCommandExecuted);
      StopServicesCommand = new RelayCommand(StopServicesCommandCanExecute, StopServicesCommandExecuted);
      IsEnabled = true;

      IconViewModel = new IconViewModel(ServiceInfosObservable);

      ServiceInfoViewModels.CollectionChanged += (sender, e) =>
        {
          if (e.NewItems != null) foreach (ServiceInfoViewModel item in e.NewItems) item.PropertyChanged += ServiceInfoViewModelChanged;

          if (e.OldItems != null) foreach (ServiceInfoViewModel item in e.OldItems) item.PropertyChanged -= ServiceInfoViewModelChanged;
        };
    }

    public IconViewModel IconViewModel { get; set; }

    public ObservableCollection<ServiceInfoViewModel> ServiceInfoViewModels { get; set; }

    public Subject<List<ServiceInfo>> ServiceInfosObservable { get; set; } = new Subject<List<ServiceInfo>>();

    public RelayCommand SearchCommand { get; set; }

    public RelayCommand StartServicesCommand { get; set; }

    public RelayCommand StopServicesCommand { get; set; }

    public string SearchString
    {
      get
      {
        return searchString;
      }

      set
      {
        searchString = value;
        OnPropertyChanged();
        SearchCommand.RaiseCanExecuteChanged();
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

    private void SearchCommandExecuted()
    {
      ServiceInfoViewModels.Clear();

      serviceHandler.Subscribe(SearchString).Subscribe(
        services =>
          {
            services.ToList().ForEach(
              service =>
                {
                  var myService = ServiceInfoViewModels.SingleOrDefault(s => s.Name == service.Name);
                  if (myService == null)
                  {
                    ServiceInfoViewModels.Add(
                      new ServiceInfoViewModel(serviceHandler) { Name = service.Name, State = service.State });
                  }
                  else
                  {
                    myService.State = service.State;
                  }
                });
          });
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
        await serviceHandler.StartServicesAsync(EnabledServices());
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

    private bool StartServicesCommandCanExecute()
    {
      return IsEnabled;
    }

    private async void StopServicesCommandExecuted()
    {
      try
      {
        IsEnabled = false;
        await serviceHandler.StopServicesAsync(EnabledServices());
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

    private bool StopServicesCommandCanExecute()
    {
      return IsEnabled;
    }

    private void ServiceInfoViewModelChanged(object sender, PropertyChangedEventArgs e)
    {
      ServiceInfosObservable.OnNext(
        ServiceInfoViewModels.Select(s => new ServiceInfo { Name = s.Name, State = s.State, Enabled = s.Enabled })
          .ToList());
    }

    private List<string> EnabledServices()
    {
      return ServiceInfoViewModels.Where(si => si.Enabled).Select(s => s.Name).ToList();
    }
  }
}