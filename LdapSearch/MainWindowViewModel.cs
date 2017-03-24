using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LdapSearch.Tests")]

namespace LdapSearch
{
  public class MainWindowViewModel : ViewModelBase
  {
    private readonly IServiceHandler serviceHandler;
    
    private string searchString;

    public MainWindowViewModel(IServiceHandler serviceHandler)
    {
      this.serviceHandler = serviceHandler;
      this.MyServices = new ObservableCollection<MyService>();
      SearchCommand = new RelayCommand(SearchCommandCanExecute, SearchCommandExecuted);
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
      serviceHandler.GetServices(SearchString).ToList().ForEach(s => MyServices.Add(new MyService { Name = s }));
    }

    private bool SearchCommandCanExecute()
    {
      return true;
    }
  }
}
