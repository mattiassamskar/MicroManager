using System.Windows.Media.Imaging;

namespace LdapSearch
{
  public interface IImageHandler
  {
    BitmapImage ConvertBytesToBitmapImage(byte[] bytes);
  }
}