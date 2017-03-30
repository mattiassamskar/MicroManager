using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace MicroManager
{
  public interface IServiceHandler
  {
    Subject<ServiceInfo> ServiceInfoObservable { get; set; }

    void RegisterEventWatcher();

    void UnRegisterEventWatcher();

    IEnumerable<ServiceInfo> GetServices(string filter);

    Task StartServicesAsync(List<string> names);

    Task StopServicesAsync(List<string> names);

    void StopService(string name);

    void StartService(string name);
  }
}