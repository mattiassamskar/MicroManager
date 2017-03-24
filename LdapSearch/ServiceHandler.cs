using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace LdapSearch
{
  using System.Reactive.Subjects;

  public class ServiceHandler : IDisposable, IServiceHandler
  {
    public Subject<MyService> MyServiceObservable { get; set; }

    private ManagementEventWatcher _eventWatcher;

    public void RegisterEventWatcher()
    {
      MyServiceObservable = new Subject<MyService>();

      _eventWatcher =
        new ManagementEventWatcher(
          new WqlEventQuery(
            "SELECT * FROM __InstanceModificationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Service'"));
      _eventWatcher.EventArrived += EventWatcherOnEventArrived;
      _eventWatcher.Start();
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
      _eventWatcher?.Stop();
    }
  }
}