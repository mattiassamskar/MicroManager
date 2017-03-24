using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace LdapSearch
{
  using System.Reactive.Subjects;

  public interface IServiceHandler
  {
    Subject<MyService> MyServiceObservable { get; set; }

    void RegisterEventWatcher();

    IEnumerable<MyService> GetServices(string filter);

    void StartServices();

    void StopServices();
  }

  public class ServiceHandler : IDisposable, IServiceHandler
  {
    public Subject<MyService> MyServiceObservable { get; set; }

    private ManagementEventWatcher eventWatcher;

    public void RegisterEventWatcher()
    {
      MyServiceObservable = new Subject<MyService>();

      eventWatcher =
        new ManagementEventWatcher(
          new WqlEventQuery(
            "SELECT * FROM __InstanceModificationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Service'"));
      eventWatcher.EventArrived += EventWatcherOnEventArrived;
      eventWatcher.Start();
  }

    private void EventWatcherOnEventArrived(object sender, EventArrivedEventArgs eventArrivedEventArgs)
    {
      var myService = new MyService
                        {
                          Name =
                            ((ManagementBaseObject)
                              eventArrivedEventArgs.NewEvent.Properties["TargetInstance"].Value)["Name"]
                            .ToString(),
                          Status =
                            ((ManagementBaseObject)
                              eventArrivedEventArgs.NewEvent.Properties["TargetInstance"].Value)["State"]
                            .ToString()
                        };

      MyServiceObservable.OnNext(myService);
    }

    public IEnumerable<MyService> GetServices(string filter)
    {
      var windowsServicesSearcher = new ManagementObjectSearcher(
        "root\\cimv2",
        $"select * from Win32_Service where Name like '%{filter}%'");
      var managementObjectCollection = windowsServicesSearcher.Get();

      return from ManagementBaseObject managementBaseObject in managementObjectCollection
             select
             new MyService
               {
                 Name = managementBaseObject["Name"].ToString(),
                 Status = managementBaseObject["State"].ToString()
               };
    }

    public void StartServices()
    {
      throw new NotImplementedException();
    }

    public void StopServices()
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      eventWatcher?.Stop();
    }
  }

  public class MyService
  {
    public string Name { get; set; }

    public string Status { get; set; }
  }
}