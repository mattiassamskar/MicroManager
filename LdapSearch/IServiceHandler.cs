using System.Collections.Generic;
using System.Reactive.Subjects;

namespace LdapSearch
{
  public interface IServiceHandler
  {
    Subject<MyService> MyServiceObservable { get; set; }

    void RegisterEventWatcher();

    IEnumerable<MyService> GetServices(string filter);

    void StartServices();

    void StopServices();
  }
}