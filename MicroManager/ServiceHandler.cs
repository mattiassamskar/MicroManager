using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reactive.Subjects;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace MicroManager
{
  public class ServiceHandler : IServiceHandler
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

    public async Task StartServicesAsync(List<string> names)
    {
      await Task.Run(() =>
      {
        Task.WaitAll(names.Select(name => Task.Run(() =>
        {
          StartService(name);
        })).ToArray());
      });
    }

    public async Task StopServicesAsync(List<string> names)
    {
      await Task.Run(() =>
      {
        Task.WaitAll(names.Select(name => Task.Run(() =>
        {
          names.ForEach(StopService);
        })).ToArray());
      });
    }

    public void StartService(string name)
    {
      var service = new ServiceController(name);

      if (service.Status != ServiceControllerStatus.Stopped)
        return;

      service.Start();
      service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
    }

    public void StopService(string name)
    {
      var service = new ServiceController(name);

      if (service.Status != ServiceControllerStatus.Running)
        return;

      service.Stop();
      service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
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

    public void UnRegisterEventWatcher()
    {
      _eventWatcher.EventArrived -= EventWatcherOnEventArrived;
      _eventWatcher.Stop();
      _eventWatcher.Dispose();
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