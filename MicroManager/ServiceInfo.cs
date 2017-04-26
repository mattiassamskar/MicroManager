using System.Management;

namespace MicroManager
{
  public class ServiceInfo
  {
    public ServiceInfo(ManagementBaseObject managementBaseObject)
    {
      Name = managementBaseObject["Name"].ToString();
      State = managementBaseObject["State"].ToString();
    }

    public string Name { get; set; }

    public string State { get; set; }

    public bool Included { get; set; }
  }
}