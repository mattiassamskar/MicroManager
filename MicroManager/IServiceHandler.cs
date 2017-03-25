using System.Collections.Generic;
using System.Reactive.Subjects;

namespace MicroManager
{
  public interface IServiceHandler
  {
    Subject<ServiceInfo> ServiceInfoObservable { get; set; }

    void RegisterEventWatcher();

    IEnumerable<ServiceInfo> GetServices(string filter);

    void StartServices(List<string> names);

    void StopServices(List<string> names);

    void StopService(string name);

    void StartService(string name);
  }
}