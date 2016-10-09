using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace LdapSearch
{
  public class User
  {
    public string DistinguishedName { get; set; }
    public string SamAccountName { get; set; }
    public string Cn { get; set; }
    public string DisplayName { get; set; }
    public BitmapImage Image { get; set; }
    public List<string> MemberOf { get; set; }
  }
}