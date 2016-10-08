using System.Collections.Generic;

namespace LdapSearch
{
  public interface ILdapHandler
  {
    IEnumerable<User> GetUsers(string searchString);
  }
}