using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reactive.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace MicroManager
{
  using System.Reactive.Subjects;

  public class ServiceHandler : IServiceHandler
  {
    private ManagementEventWatcher EventWatcher { get; set; }

    private Subject<IEnumerable<ServiceInfo>> ServiceInfosObservable { get; } = new Subject<IEnumerable<ServiceInfo>>();

    public IObservable<IEnumerable<ServiceInfo>> Subscribe(string filter)
    {
      var windowsServicesSearcher = new ManagementObjectSearcher(
        "root\\cimv2",
        $"select * from Win32_Service where Name like '%{filter}%'");

      var serviceInfos =
        windowsServicesSearcher.Get()
          .Cast<ManagementBaseObject>()
          .ToList()
          .Select(o => new ServiceInfo { Name = o["Name"].ToString(), State = o["State"].ToString() });

      EventWatcher =
        new ManagementEventWatcher(
          new WqlEventQuery(
            $"select * from __InstanceModificationEvent within 1 where TargetInstance isa 'Win32_Service' and TargetInstance.Name like '%{filter}%'"));
      EventWatcher.EventArrived += EventWatcherOnEventArrived;
      EventWatcher.Start();

      return ServiceInfosObservable.Publish(serviceInfos).RefCount();
    }

    public async Task StartServicesAsync(List<string> names)
    {
      await Task.Run(() =>
      {
        Task.WaitAll(names.Select(StartServiceAsync).ToArray());
      });
    }

    public async Task StopServicesAsync(List<string> names)
    {
      await Task.Run(() =>
      {
        Task.WaitAll(names.Select(StopServiceAsync).ToArray());
      });
    }

    public async Task StartServiceAsync(string name)
    {
      var service = new ServiceController(name);

      if (service.Status != ServiceControllerStatus.Stopped)
        return;

      await Task.Run(
        () =>
          {
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
          });
    }

    public async Task StopServiceAsync(string name)
    {
      var service = new ServiceController(name);

      if (service.Status != ServiceControllerStatus.Running)
        return;

      await Task.Run(
        () =>
          {
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
          });
    }

    private void EventWatcherOnEventArrived(object sender, EventArrivedEventArgs eventArrivedEventArgs)
    {
      var managementBaseObject = (ManagementBaseObject)
        eventArrivedEventArgs.NewEvent.Properties["TargetInstance"].Value;

      ServiceInfosObservable.OnNext(
        new List<ServiceInfo>
          {
            new ServiceInfo
              {
                Name = managementBaseObject["Name"].ToString(),
                State = managementBaseObject["State"].ToString()
              }
          });
    }
  }
}