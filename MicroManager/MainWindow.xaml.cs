using System.ComponentModel;
using System.Windows;

namespace MicroManager
{
  public partial class MainWindow : Window
  {
    private readonly IServiceHandler _serviceHandler;

    public MainWindow(MainWindowViewModel mainWindowViewModel, IServiceHandler serviceHandler)
    {
      _serviceHandler = serviceHandler;
      InitializeComponent();

      DataContext = mainWindowViewModel;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      _serviceHandler.UnRegisterEventWatcher();
      base.OnClosing(e);

    }
  }
}
