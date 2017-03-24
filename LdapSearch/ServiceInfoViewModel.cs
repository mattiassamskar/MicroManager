namespace LdapSearch
{
  public class ServiceInfoViewModel : ViewModelBase
  {
    private string _name;
    private string _state;

    public string Name
    {
      get { return _name; }
      set
      {
        OnPropertyChanged();
        _name = value;
      }
    }

    public string State
    {
      get { return _state; }
      set
      {
        OnPropertyChanged();
        _state = value;
      }
    }
  }
}