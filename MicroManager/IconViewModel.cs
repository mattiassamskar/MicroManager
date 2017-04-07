using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroManager
{
  public class IconViewModel : ViewModelBase
  {
    private string applicationIcon;

    public IconViewModel(IObservable<List<ServiceInfo>> serviceInfosObservable)
    {
      serviceInfosObservable.Subscribe(
        serviceInfos =>
          {
            if (!serviceInfos.Any())
            {
              ApplicationIcon = "MicroManager.ico";
            }
            else
            {
              ApplicationIcon = serviceInfos.Where(si => si.Enabled).All(si => si.State == "Running")
                                  ? "MicroManager-Green.ico"
                                  : "MicroManager-Red.ico";
            }
          });
    }

    public string ApplicationIcon
    {
      get
      {
        return applicationIcon;
      }
      set
      {
        applicationIcon = value;
        OnPropertyChanged();
      }
    }
  }
}