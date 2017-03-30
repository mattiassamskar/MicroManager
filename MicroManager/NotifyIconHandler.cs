using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MicroManager
{
  public class NotifyIconHandler
  {
    private readonly NotifyIcon _notifyIcon;

    public NotifyIconHandler(IObservable<List<ServiceInfo>> serviceInfosobservable)
    {
      _notifyIcon = new NotifyIcon
      {
        Visible = true
      };

      serviceInfosobservable.Subscribe(serviceInfos =>
      {
        if (!serviceInfos.Any())
          _notifyIcon.Icon = Resource1.grey;
        else if (serviceInfos.Where(s => s.Enabled).All(s => s.State == "Running"))
          _notifyIcon.Icon = Resource1.green;
        else
        {
          _notifyIcon.Icon = Resource1.red;
        }
      });
    }
  }
}