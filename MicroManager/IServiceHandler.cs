using System.Collections.Generic;
using System.Reactive.Subjects;

namespace MicroManager
{
  public interface IServiceHandler
  {
    Subject<ServiceInfo> ServiceInfosObservable { get; }

    void StartService(string name);

    void StopService(string name);

    void ToggleService(string name);

    IEnumerable<ServiceInfo> GetServiceInfos(string filter);
  }
}