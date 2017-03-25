using System.Windows;
using MicroManager;

namespace LdapSearch
{
  public partial class MainWindow : Window
  {
    public MainWindow(MainWindowViewModel mainWindowViewModel)
    {
      InitializeComponent();

      DataContext = mainWindowViewModel;
    }
  }
}
