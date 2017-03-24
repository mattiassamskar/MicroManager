using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LdapSearch.Tests")]

namespace LdapSearch
{
  using System;

  public class MainWindowViewModel : ViewModelBase
  {
    private readonly IServiceHandler serviceHandler;
    
    private string searchString;

    public MainWindowViewModel(IServiceHandler serviceHandler)
    {
      this.serviceHandler = serviceHandler;
      this.MyServices = new ObservableCollection<MyService>();
      SearchCommand = new RelayCommand(SearchCommandCanExecute, SearchCommandExecuted);

      this.serviceHandler.RegisterEventWatcher();
      this.serviceHandler.MyServiceObservable.Subscribe(
        service =>
          {
            var myService = MyServices.SingleOrDefault(s => s.Name == service.Name);
            if (myService == null) return;
            myService.Status = service.Status; // TODO: Must notify ui
          });
    }

    public ObservableCollection<MyService> MyServices { get; set; }

    public RelayCommand SearchCommand { get; set; }

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

    internal void SearchCommandExecuted()
    {
      if (string.IsNullOrEmpty(SearchString)) return;

      MyServices.Clear();
      serviceHandler.GetServices(SearchString).ToList().ForEach(s => MyServices.Add(s));
    }

    private bool SearchCommandCanExecute()
    {
      return true;
    }
  }
}
