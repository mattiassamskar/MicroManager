﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reactive.Subjects;
using System.ServiceProcess;

namespace MicroManager
{
  public class ServiceHandler : IServiceHandler
  {
    private ManagementEventWatcher EventWatcher { get; }

    public Subject<ServiceInfo> ServiceInfosObservable { get; } = new Subject<ServiceInfo>();

    public ServiceHandler()
    {
      EventWatcher =
        new ManagementEventWatcher(
          new WqlEventQuery(
            "select * from __InstanceModificationEvent within 1 " + "where TargetInstance isa 'Win32_Service'"));

      EventWatcher.EventArrived += (sender, eventArrivedEventArgs) =>
      {
        var managementBaseObject =
          (ManagementBaseObject)eventArrivedEventArgs.NewEvent.Properties["TargetInstance"].Value;

        ServiceInfosObservable.OnNext(new ServiceInfo(managementBaseObject));
      };

      EventWatcher.Start();
    }

    public void StartService(string name)
    {
      var service = new ServiceController(name);

      if (service.Status != ServiceControllerStatus.Stopped) return;

      service.Start();
      service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(20));
    }

    public void StopService(string name)
    {
      var service = new ServiceController(name);

      if (service.Status != ServiceControllerStatus.Running) return;

      service.Stop();
      service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(20));
    }

    public void ToggleService(string name)
    {
      var service = new ServiceController(name);

      switch (service.Status)
      {
          case ServiceControllerStatus.Running:
            StopService(name);
            break;
          case ServiceControllerStatus.Stopped:
            StartService(name);
            break;
      }
    }

    public IEnumerable<ServiceInfo> GetServiceInfos(string filter)
    {
      var managementObjectSearcher = new ManagementObjectSearcher(
        "root\\cimv2",
        $"select * from Win32_Service where Name like '%{filter}%'");

      return
        managementObjectSearcher.Get()
          .Cast<ManagementBaseObject>()
          .Select(managementBaseObject => new ServiceInfo(managementBaseObject));
    }
  }
}