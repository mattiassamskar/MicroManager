using System;
using System.Collections.Generic;
using System.Management;
using System.Reactive.Subjects;

namespace LdapSearch
{
  public class ServiceHandler : IDisposable, IServiceHandler
  {
    public Subject<ServiceInfo> ServiceInfoObservable { get; set; } = new Subject<ServiceInfo>();

    private ManagementEventWatcher _eventWatcher;

    public IEnumerable<ServiceInfo> GetServices(string filter)
    {
      var windowsServicesSearcher = new ManagementObjectSearcher(
        "root\\cimv2",
        $"select * from Win32_Service where Name like '%{filter}%'");
      var managementObjectCollection = windowsServicesSearcher.Get();

      foreach (var managementBaseObject in managementObjectCollection)
        yield return new ServiceInfo
        {
          Name = managementBaseObject["Name"].ToString(),
          State = managementBaseObject["State"].ToString()
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

    public void RegisterEventWatcher()
    {
      _eventWatcher =
        new ManagementEventWatcher(
          new WqlEventQuery(
            "SELECT * FROM __InstanceModificationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Service'"));
      _eventWatcher.EventArrived += EventWatcherOnEventArrived;
      _eventWatcher.Start();
    }

    public void Dispose()
    {
      _eventWatcher?.Stop();
    }

    private void EventWatcherOnEventArrived(object sender, EventArrivedEventArgs eventArrivedEventArgs)
    {
      var managementBaseObject = (ManagementBaseObject)
        eventArrivedEventArgs.NewEvent.Properties["TargetInstance"].Value;

      var myService = new ServiceInfo
      {
        Name = managementBaseObject["Name"].ToString(),
        State = managementBaseObject["State"].ToString()
      };

      ServiceInfoObservable.OnNext(myService);
    }
  }
}