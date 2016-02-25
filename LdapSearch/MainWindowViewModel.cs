using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LdapSearch.Tests")]

namespace LdapSearch
{
  public class MainWindowViewModel : ViewModelBase
  {
    private readonly ILdapHandler ldapHandler_;
    
    private string searchString;
    private User selectedUser;

    public MainWindowViewModel(ILdapHandler ldapHandler)
    {
      ldapHandler_ = ldapHandler;
      Users = new ObservableCollection<User>();
      SearchCommand = new RelayCommand(SearchCommandCanExecute, SearchCommandExecuted);
    }

    public ObservableCollection<User> Users { get; set; }

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

    public User SelectedUser
    {
      get
      {
        return selectedUser;
      }
      set
      {
        selectedUser = value;
        OnPropertyChanged();
      }
    }

    internal void SearchCommandExecuted()
    {
      if (string.IsNullOrEmpty(SearchString)) return;

      Users.Clear();
      var searchStrings = SearchString.Split(new[] { ';', ',' }).ToList();
      searchStrings.ForEach(s => ldapHandler_.Search(s).ToList().ForEach(Users.Add));
      SelectedUser = Users.FirstOrDefault();
    }

    private bool SearchCommandCanExecute()
    {
      return true;
    }
  }
}
