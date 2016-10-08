using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace LdapSearch
{
  public class LdapHandler : ILdapHandler
  {
    private readonly IImageHandler _imageHandler;

    public LdapHandler(IImageHandler imageHandler)
    {
      _imageHandler = imageHandler;
    }

    public IEnumerable<User> GetUsers(string searchString)
    {
      using (var directorySearcher = new DirectorySearcher(new DirectoryEntry()))
      {
        directorySearcher.Filter = "(|(sAMAccountName=" + searchString + ")(cn=" + searchString + "))";
        directorySearcher.PropertiesToLoad.AddRange(new[] { "cn", "sAMAccountName", "displayName", "distinguishedName", "thumbnailPhoto", "memberOf" });
        return directorySearcher.FindAll().Cast<SearchResult>().Select(CreateUserFromSearchResult);
      }
    }

    private User CreateUserFromSearchResult(SearchResult searchResult)
    {
      return new User
      {
        Cn = searchResult.SafeGetProperty<string>("cn"),
        SamAccountName = searchResult.SafeGetProperty<string>("sAMAccountName"),
        DisplayName = searchResult.SafeGetProperty<string>("displayName"),
        DistinguishedName = searchResult.SafeGetProperty<string>("distinguishedName"),
        Image = _imageHandler.ConvertBytesToBitmapImage(searchResult.SafeGetProperty<byte[]>("thumbnailPhoto")),
        MemberOf = GetNamesFromDistinguishedNames(searchResult.SafeGetListProperty<string>("memberOf")).ToList()
      };
    }

    private IEnumerable<string> GetNamesFromDistinguishedNames(IEnumerable<string> distinguishedNames)
    {
      using (var directorySearcher = new DirectorySearcher(new DirectoryEntry()))
      {
        directorySearcher.PropertiesToLoad.Add("name");
        return distinguishedNames.Select(distinguishedName =>
        {
          directorySearcher.Filter = "distinguishedName=" + distinguishedName;
          return directorySearcher.FindOne().SafeGetProperty<string>("name");
        }).ToList();
      }
    }
  }
}