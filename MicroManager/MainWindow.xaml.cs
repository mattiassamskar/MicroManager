using System;
using System.ComponentModel;
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

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);

      ((MainWindowViewModel) DataContext).Scale = Settings.Default.Scale;
      ((MainWindowViewModel) DataContext).TopMost = Settings.Default.TopMost;
      WindowPlacementHandler.LoadWindowPlacement(this);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      Settings.Default.Scale = ((MainWindowViewModel) DataContext).Scale;
      Settings.Default.TopMost = ((MainWindowViewModel) DataContext).TopMost;
      WindowPlacementHandler.SaveWindowPlacement(this);
    }
  }
}

