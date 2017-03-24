using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LdapSearch.Tests")]

namespace LdapSearch
{
  using System;

  public class MainWindowViewModel : ViewModelBase
  {
    private readonly IServiceHandler _serviceHandler;
    
    private string _searchString;

    public MainWindowViewModel(IServiceHandler serviceHandler)
    {
      _serviceHandler = serviceHandler;
      MyServices = new ObservableCollection<MyService>();
      SearchCommand = new RelayCommand(SearchCommandCanExecute, SearchCommandExecuted);

      _serviceHandler.RegisterEventWatcher();
      _serviceHandler.MyServiceObservable.Subscribe(
        service =>
          {
            var myService = MyServices.SingleOrDefault(s => s.Name == service.Name);
            if (myService == null) return;
            myService.Status = service.Status;
          });
    }

    public ObservableCollection<MyService> MyServices { get; set; }

    public RelayCommand SearchCommand { get; set; }

    public string SearchString
    {
      get
      {
        return _searchString;
      }
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

      MyServices.Clear();
      _serviceHandler.GetServices(SearchString).ToList().ForEach(s => MyServices.Add(s));
    }

    private bool SearchCommandCanExecute()
    {
      return true;
    }
  }
}
