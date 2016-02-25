using System.Collections.Generic;

namespace LdapSearch
{
  public interface ILdapHandler
  {
    IEnumerable<User> Search(string searchString);
  }
}