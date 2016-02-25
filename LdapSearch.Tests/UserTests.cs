using Xunit;

namespace LdapSearch.Tests
{
  public class UserTests
  {
    [Fact]
    public void New_user_memberOf_list_should_not_be_null()
    {
      // Arrange
      var user = new User();

      // Act
      // Assert
      Assert.NotNull(user.MemberOf);
    }
  }
}
