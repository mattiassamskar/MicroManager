using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace MicroManager
{
  public interface IServiceHandler
  {
    Subject<ServiceInfoViewModel.ServiceInfo> ServiceInfosObservable { get; }

    Task StartServiceAsync(string name);

    Task StopServiceAsync(string name);

    IEnumerable<ServiceInfoViewModel.ServiceInfo> GetServiceInfos(string filter);
  }
}