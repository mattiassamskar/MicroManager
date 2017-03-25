using System;
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
  }
}