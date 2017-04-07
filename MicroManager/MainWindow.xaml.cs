using System.Windows;

namespace MicroManager
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
