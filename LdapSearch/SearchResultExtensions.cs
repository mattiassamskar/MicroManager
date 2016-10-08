using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace LdapSearch
{
  public static class SearchResultExtensions
  {
    public static bool HasProperty(this SearchResult searchResult, string propertyName)
    {
      return searchResult.Properties.Contains(propertyName);
    }

    public static T SafeGetProperty<T>(this SearchResult searchResult, string propertyName)
    {
      return searchResult.HasProperty(propertyName) ? (T)searchResult.Properties[propertyName][0] : default(T);
    }

    public static IEnumerable<T> SafeGetListProperty<T>(this SearchResult searchResult, string propertyName)
    {
      return searchResult.HasProperty(propertyName) ? searchResult.Properties[propertyName].Cast<T>() : new List<T>();
    }
  }
}