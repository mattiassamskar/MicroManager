using System.Collections.Generic;
using System.Reactive.Subjects;

namespace MicroManager
{
  public interface IServiceHandler
  {
    Subject<ServiceInfo> ServiceInfoObservable { get; set; }

    void RegisterEventWatcher();

    IEnumerable<ServiceInfo> GetServices(string filter);

    void StartServices();

    void StopServices();
  }
}