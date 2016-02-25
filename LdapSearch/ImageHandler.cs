using System.IO;
using System.Windows.Media.Imaging;

namespace LdapSearch
{
  public class ImageHandler : IImageHandler
  {
    public BitmapImage ConvertBytesToBitmapImage(byte[] bytes)
    {
      if (bytes == null) return null;

      using (var memoryStream = new MemoryStream(bytes))
      {
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = memoryStream;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        return image;
      }
    }
  }
}