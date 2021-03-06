﻿using System;
using SimpleInjector;

namespace MicroManager
{
  public class Program
  {
    [STAThread]
    static void Main()
    {
      var container = Bootstrap();
      RunApplication(container);
    }

    private static Container Bootstrap()
    {
      var container = new Container();

      container.Register<MainWindow>();
      container.Register<MainWindowViewModel>();
      container.Register<IServiceHandler, ServiceHandler>(Lifestyle.Singleton);

      container.Verify();

      return container;
    }

    private static void RunApplication(Container container)
    {
      var app = new App();
      var mainWindow = container.GetInstance<MainWindow>();
      app.Run(mainWindow);
    }
  }
}
