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

    public Subject<ServiceInfoViewModel.ServiceInfo> ServiceInfosObservable { get; } =
      new Subject<ServiceInfoViewModel.ServiceInfo>();

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

    public IEnumerable<ServiceInfoViewModel.ServiceInfo> GetServiceInfos(string filter)
    {
      EventWatcher =
        new ManagementEventWatcher(
          new WqlEventQuery(
            "select * from __InstanceModificationEvent within 1 " + "where TargetInstance isa 'Win32_Service'"));

      EventWatcher.EventArrived += (sender, eventArrivedEventArgs) =>
        {
          var managementBaseObject =
            (ManagementBaseObject)eventArrivedEventArgs.NewEvent.Properties["TargetInstance"].Value;

          ServiceInfosObservable.OnNext(
            new ServiceInfoViewModel.ServiceInfo
              {
                Name = managementBaseObject["Name"].ToString(),
                State = managementBaseObject["State"].ToString()
              });
        };
      EventWatcher.Start();

      ServiceInfosObservable.Publish().RefCount();

      var managementObjectSearcher = new ManagementObjectSearcher(
        "root\\cimv2",
        $"select * from Win32_Service where Name like '%{filter}%'");

      return
        managementObjectSearcher.Get()
          .Cast<ManagementBaseObject>()
          .Select(
            o => new ServiceInfoViewModel.ServiceInfo { Name = o["Name"].ToString(), State = o["State"].ToString() });
    }
  }
}