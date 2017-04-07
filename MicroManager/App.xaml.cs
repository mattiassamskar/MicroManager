using System.Windows;

namespace MicroManager
{
  public partial class App : Application
  {
    public App()
    {
      Dispatcher.UnhandledException += (sender, args) =>
        {
          var message = args.Exception.Message + " " + args.Exception.InnerException?.Message;
          MessageBox.Show(message, "MicroManager", MessageBoxButton.OK, MessageBoxImage.Error);
        };
    }
  }
}
