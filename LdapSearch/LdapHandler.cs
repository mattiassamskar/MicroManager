using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace LdapSearch
{
  public class LdapHandler : ILdapHandler
  {
    private readonly IImageHandler imageHandler_;

    public LdapHandler(IImageHandler imageHandler)
    {
      imageHandler_ = imageHandler;
    }

    public IEnumerable<User> Search(string searchString)
    {
      var users = new List<User>();

      using (var directorySearcher = CreateDirectorySearcher(searchString))
      {
        foreach (SearchResult searchResult in directorySearcher.FindAll())
        {
          var user = new User
                       {
                         Cn = searchResult.SafeGetProperty<string>("cn"),
                         SamAccountName = searchResult.SafeGetProperty<string>("sAMAccountName"),
                         DisplayName = searchResult.SafeGetProperty<string>("displayName"),
                         DistinguishedName = searchResult.SafeGetProperty<string>("distinguishedName"),
                         Image = imageHandler_.ConvertBytesToBitmapImage(searchResult.SafeGetProperty<byte[]>("thumbnailPhoto")),
                       };

          GetLdapNames(searchResult.SafeGetListProperty<string>("memberOf"))
            .ToList()
            .ForEach(user.MemberOf.Add);

          users.Add(user);
        }
      }
      return users;
    }

    private DirectorySearcher CreateDirectorySearcher(string searchString)
    {
      var directorySearcher = new DirectorySearcher(new DirectoryEntry());
      directorySearcher.Filter = "(|(sAMAccountName=" + searchString + ")(cn=" + searchString + "))";
      directorySearcher.PropertiesToLoad.Add("cn");
      directorySearcher.PropertiesToLoad.Add("sAMAccountName");
      directorySearcher.PropertiesToLoad.Add("displayName");
      directorySearcher.PropertiesToLoad.Add("distinguishedName");
      directorySearcher.PropertiesToLoad.Add("thumbnailPhoto");
      directorySearcher.PropertiesToLoad.Add("memberOf");

      return directorySearcher;
    }

    private IEnumerable<string> GetLdapNames(IEnumerable distinguishedNames)
    {
      return distinguishedNames != null ? distinguishedNames.Cast<string>().Select(GetLdapName) : new string[0];
    }

    private string GetLdapName(string distinguishedName)
    {
      using (var entry = new DirectoryEntry())
      using (var directorySearcher = new DirectorySearcher(entry) { Filter = "distinguishedName=" + distinguishedName })
      {
        directorySearcher.PropertiesToLoad.Add("name");
        var searchResult = directorySearcher.FindOne();
        return searchResult.SafeGetProperty<string>("name");
      }
    }
  }
}