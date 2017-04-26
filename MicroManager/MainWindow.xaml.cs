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

      WindowPlacementHandler.LoadWindowPlacement(this);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      WindowPlacementHandler.SaveWindowPlacement(this);
    }
  }
}

