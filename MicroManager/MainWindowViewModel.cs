using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

[assembly: InternalsVisibleTo("MicroManager.Tests")]

namespace MicroManager
{
  public class MainWindowViewModel : ViewModelBase
  {
    private readonly IServiceHandler _serviceHandler;
    private string _searchString;
    private bool _isEnabled = true;

    public MainWindowViewModel(IServiceHandler serviceHandler)
    {
      _serviceHandler = serviceHandler;

      SearchCommand = new RelayCommand(() => IsEnabled, SearchCommandExecuted);
      StartServicesCommand = new RelayCommand(() => IsEnabled, StartServicesCommandExecuted);
      StopServicesCommand = new RelayCommand(() => IsEnabled, StopServicesCommandExecuted);

      IconViewModel = new IconViewModel(ServiceInfosObservable);

      ServiceInfoViewModels.CollectionChanged += (sender, e) =>
      {
        if (e.NewItems != null)
          foreach (ServiceInfoViewModel item in e.NewItems) item.PropertyChanged += ServiceInfoViewModelChanged;

        if (e.OldItems != null)
          foreach (ServiceInfoViewModel item in e.OldItems) item.PropertyChanged -= ServiceInfoViewModelChanged;

        UpdateServiceInfosObservable();
      };
    }

    public IconViewModel IconViewModel { get; set; }

    public ObservableCollection<ServiceInfoViewModel> ServiceInfoViewModels { get; set; } =
      new ObservableCollection<ServiceInfoViewModel>();

    public Subject<List<ServiceInfoViewModel.ServiceInfo>> ServiceInfosObservable { get; set; } =
      new Subject<List<ServiceInfoViewModel.ServiceInfo>>();

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
      get { return _isEnabled; }

      set
      {
        _isEnabled = value;
        OnPropertyChanged();
      }
    }

    private void SearchCommandExecuted()
    {
      ServiceInfoViewModels.Clear();

      _serviceHandler.WhenServiceInfosChange(SearchString).Subscribe(
        services =>
        {
          services.ToList().ForEach(
            service =>
            {
              var myService = ServiceInfoViewModels.SingleOrDefault(s => s.Name == service.Name);
              if (myService == null)
              {
                ServiceInfoViewModels.Add(
                  new ServiceInfoViewModel(_serviceHandler) {Name = service.Name, State = service.State});
              }
              else
              {
                myService.State = service.State;
              }
            });
        });
    }

    private async void StartServicesCommandExecuted()
    {
      try
      {
        IsEnabled = false;
        await
          Task.Run(
            () =>
            {
              Task.WaitAll(
                ServiceInfoViewModels.Where(si => si.Included)
                  .Select(s => _serviceHandler.StartServiceAsync(s.Name))
                  .ToArray());
            });
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

    private async void StopServicesCommandExecuted()
    {
      try
      {
        IsEnabled = false;
        await
          Task.Run(
            () =>
            {
              Task.WaitAll(
                ServiceInfoViewModels.Where(si => si.Included)
                  .Select(s => _serviceHandler.StopServiceAsync(s.Name))
                  .ToArray());
            });
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

    private void ServiceInfoViewModelChanged(object sender, PropertyChangedEventArgs e)
    {
      UpdateServiceInfosObservable();
    }

    private void UpdateServiceInfosObservable()
    {
      ServiceInfosObservable.OnNext(
        ServiceInfoViewModels.Select(
            s => s._serviceInfo)
          .ToList());
    }
  }
}