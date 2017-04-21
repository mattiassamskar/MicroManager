using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroManager
{
  public interface IServiceHandler
  {
    Task StartServiceAsync(string name);

    Task StopServiceAsync(string name);

    IObservable<IEnumerable<ServiceInfoViewModel.ServiceInfo>> WhenServiceInfosChange(string filter);
  }
}