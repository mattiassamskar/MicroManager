using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroManager
{
  public interface IServiceHandler
  {
    IObservable<IEnumerable<ServiceInfo>> Subscribe(string filter);

    Task StartServicesAsync(List<string> names);

    Task StopServicesAsync(List<string> names);

    Task StartServiceAsync(string name);

    Task StopServiceAsync(string name);
  }
}