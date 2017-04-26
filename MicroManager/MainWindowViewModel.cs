using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

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
      };
    }

    public IconViewModel IconViewModel { get; set; }

    public ObservableCollection<ServiceInfoViewModel> ServiceInfoViewModels { get; set; } =
      new ObservableCollection<ServiceInfoViewModel>();

    public Subject<IEnumerable<string>> ServiceInfosObservable { get; } = new Subject<IEnumerable<string>>();

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

      _serviceHandler.GetServiceInfos(SearchString)
        .ToList()
        .ForEach(s => ServiceInfoViewModels.Add(new ServiceInfoViewModel(_serviceHandler, s)));

      UpdateServiceInfosObservable();
    }

    private async void StartServicesCommandExecuted()
    {
      IsEnabled = false;
      await Task.WhenAll(ServiceInfoViewModels.Where(s => s.Included).Select(s => s.StartServiceAsync()).ToArray());
      IsEnabled = true;
    }

    private async void StopServicesCommandExecuted()
    {
      IsEnabled = false;
      await Task.WhenAll(ServiceInfoViewModels.Where(s => s.Included).Select(s => s.StopServiceAsync()).ToArray());
      IsEnabled = true;
    }

    private void ServiceInfoViewModelChanged(object sender, PropertyChangedEventArgs e)
    {
      UpdateServiceInfosObservable();
    }

    private void UpdateServiceInfosObservable()
    {
      ServiceInfosObservable.OnNext(ServiceInfoViewModels.Where(s => s.Included).Select(s => s.State));
    }
  }
}