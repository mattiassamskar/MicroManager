using Xunit;

namespace LdapSearch.Tests
{
  public class ImageHandlerTests
  {
    [Fact]
    public void No_bytes_should_return_null()
    {
      // Arrange
      var imageHandler = new ImageHandler();

      // Act
      var image = imageHandler.ConvertBytesToBitmapImage(null);

      // Assert
      Assert.Null(image);
    }
  }
}
